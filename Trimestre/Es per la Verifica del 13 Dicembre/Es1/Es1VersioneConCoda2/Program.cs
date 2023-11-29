namespace Es1VersioneConCoda2
{
    internal class Program
    {
        //Es. N1
        //Una giostra ha una capienza di 5 persone.Scrivi un programma, che mediante l’utilizzo dei semafori, gestisca l’accesso delle persone alla giostra.
        //Quando è il turno di una persona, essa sale sulla giostra per un periodo di tempo casuale compreso tra 1 e 3 secondi,
        //alla fine dei quali libera il posto al prossimo.
        //Il programma stampa su console:
        //- la persona x si mette in coda(appena creato il Thread)
        //- la persona x sale sulla giostra(quando entra in sezione critica)
        //- la persona x esce dalla giostra(quando esce dalla giostra e sta per terminare)
        //Ipotiziamo che se in un bambino trova una coda di attesa di 10 persone se ne va
        //Dati condivisi
        const int MaxBimbi = 50;
        const int numeroMaxBimbiInCoda = 10;
        const int postiliberi = 5;
        static SemaphoreSlim PostiLiberi = new(postiliberi, postiliberi);
        static Random random = new Random();
        static readonly object _lock = new();
        static readonly object _lockBimbi = new();
        static int numeroBimbiInAttesa = 0;

        static void Main(string[] args)
        {
            //simuliamo l'avento dei bambini
            for (int i = 0; i < MaxBimbi; i++)
            {
                new Thread(Giostra).Start(i);
                Thread.Sleep(100);
            }
        }

        private static void Giostra(object? obj)
        {
            bool siFerma = false;
            lock(_lockBimbi)
            {
                if (numeroBimbiInAttesa < numeroMaxBimbiInCoda)
                {
                    numeroBimbiInAttesa++;
                    siFerma = true;
                    Console.WriteLine($"Numero bimbo in atessa {numeroBimbiInAttesa}");
                }
            }
            int indice = obj == null ? -1 : (int)obj;
            if (siFerma)
            {
                Console.WriteLine($"Sono il bambino {indice}-mo, con Thread Id = {Environment.CurrentManagedThreadId} e attendo di salire sulla giostra");
                PostiLiberi.Wait();
                //Quando un bambino sale si libera un posto di attesa
                lock(_lockBimbi)
                {
                    numeroBimbiInAttesa--;
                }
                Console.WriteLine($"Sono il bambino {indice}-mo, con Thread Id = {Environment.CurrentManagedThreadId} e sono sulla giostra");
                //siccome più thread potrebbero lanciare il metodo Next contemporaneamente potremmo avere interferenze, pertanto usaimo il lock
                int randomMillisecond;
                lock (_lock)
                {
                    randomMillisecond = random.Next(1000, 3001);
                }
                Thread.Sleep(randomMillisecond);
                Console.WriteLine($"Sono il bambino {indice}-mo, con Thread Id = {Environment.CurrentManagedThreadId} e scendo dalla giostra");
                PostiLiberi.Release();
            }
            else
            {
                Console.WriteLine($"Sono il bambino {indice}-mo, con Thread Id = {Environment.CurrentManagedThreadId} ho trovato troppa fila e me ne sono andato");
            }
        }
    }
}
