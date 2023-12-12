namespace AutoLavaggio
{
    internal class Program
    {
        //Autolavaggio Concorrente
        //Prima parte: Scrivere un programma multithreading console C# che simuli il funzionamento di un autolavaggio mediante Task.
        //L’autolavaggio è costituito da un tunnel nel quale possono entrare le macchine una alla volta. Davanti al tunnel c’è un
        //parcheggio nel quale possono entrare al massimo 20 macchine. Se una macchina arriva e c’è posto nel parcheggio entra e
        //aspetta di entrare nel tunnel. Se una macchina arriva e l’autolavaggio è chiuso, oppure è aperto, ma non c’è posto nel parcheggio, va via.
        //Le macchine arrivano presso l’autolavaggio con un intervallo di tempo, tra l’una e l’altra, variabile in maniera random
        //e compreso tra 0,1 e 0,3 secondi.Quando una macchina entra nel tunnel, il programma stampa “la macchina con indice i
        //su Task id = TaskId entra nel tunnel di lavaggio, ho atteso x ms da quando sono entrata”. Il tempo di permanenza di una
        //macchina nel tunnel è di 0.4 secondi.Passato questo tempo, la macchina esce dal tunnel e il programma stampa
        //“la macchina i-ma esce dal tunnel pulita e va via”.
        //Se l’autolavaggio viene chiuso, ma ci sono ancora auto in attesa di essere lavate, il sistema lava tutte le auto nel
        //parcheggio di attesa.Quando non ci sono più macchine in attesa e il parcheggio è chiuso il sistema termina la sua attività.
        //Il Main program fa partire il Task che simula il tunnel di lavaggio e 50 Task che simulano le auto che arrivano
        //presso l’autolavaggio; quindi, va in Sleep per 10 secondi, poi chiude l’autolavaggio e aspetta che il sistema
        //di lavaggio finisca il suo lavoro.

        //Seconda parte: Modificare il programma in modo che ci siano due o più tunnel nello stesso autolavaggio.
        const int NParcheggi = 20;
        const int Macchine = 50;
        const int Tunnel = 2;
        static int PostiDisponibili = NParcheggi;
        static bool stato;
        const int DurataTunnel = 400; //ms
        const int Min = 100;//ms
        const int Max = 300;//ms
        private static readonly object _lockParcheggiDisponibili = new();
        private static readonly object _lockStato = new();
        static readonly SemaphoreSlim TunnelPronto = new(Tunnel, Tunnel);//serventi
        static readonly SemaphoreSlim MacchineNelParcheggio = new(0, NParcheggi);//utenti
        static void Main(string[] args)
        {
            lock (_lockStato)
            {
                Console.WriteLine($"L'autolavaggio è aperto");
                stato = true;//apro l'autolavaggio
            }
            var tunnel = Task.Factory.StartNew(() => TunnelAction());

            Random rand = new();
            for (int i = 0; i < Macchine; i++)
            {
                Task.Factory.StartNew((object? obj) => CarAction(obj), new CarData() { Id = i, CreationTime = DateTime.Now.Ticks });
                int timeInterval = rand.Next(Min, Max + 1);//ms
                Task.Delay(timeInterval).Wait();
            }
            Console.WriteLine("Non arrivano più auto");
            //aspetto 10 secondi
            Task.Delay(10000).Wait();
            lock (_lockStato)
            {
                //chiudo l'autolavaggio
                Console.WriteLine($"L'autolavaggio è chiuso");
                stato = false;
            }
            tunnel.Wait();

        }

        private static void CarAction(object? obj)
        {
            CarData? carData = obj as CarData;
            //se il parcheggio è aperto
            bool statoTunnel;
            //
            lock (_lockStato)
            {
                statoTunnel = stato;
            }
            if(statoTunnel)
            {
                bool entro = false;
                int posto = 0;
                //occupo un posto nel parcheggio se c'è posto,
                lock (_lockParcheggiDisponibili)
                {
                    posto = PostiDisponibili;
                    PostiDisponibili--;
                    Console.WriteLine($"");
                    entro = true;
                }
                //segnalo che è aumentato il numero di auto in attesa
                //attendo che il tunnel sia disponibile
                if (entro)
                {
                    Console.WriteLine($"L'auto con indice: {carData.Id} sul Task ID: {Task.CurrentId} trova parcheggio, ci sono {posto} posti disponibili");
                    MacchineNelParcheggio.Release();
                    TunnelPronto.Wait();
                    double tempoAttesa = default;
                    if(carData!=null)
                    {
                        tempoAttesa = new TimeSpan(DateTime.Now.Ticks - carData.CreationTime).TotalMilliseconds;
                    }
                    lock( _lockStato)
                    {
                        Console.WriteLine($"Sono la macchina con indice {carData.Id} su Task id {Task.CurrentId} e ho atteso {tempoAttesa}");
                    }
                }
            }
            //stampo il tempo di attesa
            //se non c'è posto vado via
            //altrimenti vado via
        }

        private static void TunnelAction()
        {
            //fino a che l'autolavaggio è aperto
            bool statoTunnel;
            //
            lock(_lockStato)
            {
                statoTunnel = stato;
            }
            //la macchina entra nel tunnel e si libera un posto nel parcheggio
            while( statoTunnel)
            {
                MacchineNelParcheggio.Wait();
                lock(_lockParcheggiDisponibili)
                {
                    PostiDisponibili++;
                    Console.WriteLine($"Il tunnel sta lavando un'auto. Si libera un posto nel parcheggio");
                }
                //attendo il tempo di lavaggio del tunnel
                Task.Delay(DurataTunnel).Wait();
                //il tunnel si libera ed è nuovamente pronto
                TunnelPronto.Release();
                Console.WriteLine("Auto lavata, sfreccia via!!");
                lock (_lockStato)
                {
                    statoTunnel = stato;
                }
            }
        }
    }

    class CarData
    {
        public int Id { get; set; }
        public long CreationTime { get; set; }
    }
}