using System.Threading;

namespace ProblemaDelBarbiere2
{
    internal class Program
    {
        const int numerOfSeats = 10;//numero sedie di attessa
        const int NumberOfClientsInSim = 30;
        static SemaphoreSlim barberReady = new(1, 1);//Disponibilità del brabiere
        static SemaphoreSlim clientReady = new(0, numerOfSeats);
        static int freeSeats = numerOfSeats;//numero di posti di attesa ancora disponibili
        static readonly object _lock = new object();
        static void Main(string[] args)
        {
            //simulo il lavoro del barbiere -> il servente
            Thread barber = new Thread(Barber);
            barber.Start();
            for (int i = 0; i < NumberOfClientsInSim; i++)
            {
                new Thread(Client).Start();
                Thread.Sleep(500);
            }
            //Console.WriteLine($"Fine: il barbiere ha fatto {clientiFatti} e ha perso  {clientiPersi}");
        }



        private static void Client(object? obj)
        {
            //il cliente guarda se c'è posto
            //se c'è posto si siede e attende di essere servit
            //se non c'è posto se ne va
            bool siSiede = false;
            //apro la sezione critica
            lock(_lock)
            {
                if (freeSeats > 0)//c'è posto
                {
                    //il cliente si siede
                    freeSeats--;
                    Console.WriteLine($"Il cliente con Thread Id = {Environment.CurrentManagedThreadId} si siede, ci sono ancora {freeSeats} posti liberi");
                    siSiede = true;
                }
                else
                {
                    siSiede = false;
                }
            }
            if(siSiede)
            {
                clientReady.Release();
                //attendo che il barbiere si liberi
                barberReady.Wait();
                Console.WriteLine($"Il cliente con Thread Id = {Environment.CurrentManagedThreadId} sta per salire sulla poltrona di taglio");
            }
            else
            {
                Console.WriteLine($"Il cliente con Thread ID = {Environment.CurrentManagedThreadId} non trova posto e se ne va");
            }
        }

        private static void Barber(object? obj)
        {
            while (true)
            {
                //rimane in attesa di un cliente
                Console.WriteLine("Il barbiere è in attesa di un nuovo cliente");
                clientReady.Wait();
                //c'è almeno un cliente, lo fa accomodare sulla poltrona di taglio
                lock(_lock)
                {
                    freeSeats++; //si libera un posto nella sala d'attesa
                }
                Console.WriteLine("Il barbiere libera un posto");
                //il barbiere taglia i capelli
                Console.WriteLine("Il barbiere sta tagliando i capelli");
                Thread.Sleep(2000);
                //il barbiere è di nuovo disponibile
                Console.WriteLine("Il barbiere ha finito");
                barberReady.Release();
            }
        }
    }
}
