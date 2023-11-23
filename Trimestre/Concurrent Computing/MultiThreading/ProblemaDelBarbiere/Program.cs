using System.Text;
using System.Threading;

namespace ProblemaDelBarbiere
{
    internal class Program
    {
        const int numerOfSeats = 10;//numero sedie di attessa
        const int NumberOfClientsInSim = 30;
        static int clientiFatti = 0;
        static int clientiPersi = 0;
        static SemaphoreSlim barberReady = new(1, 1);//Disponibilità del brabiere
        static SemaphoreSlim clientReady = new(0, numerOfSeats);
        static int freeSeats = numerOfSeats;//numero di posti di attesa ancora disponibili
        static SemaphoreSlim seatAccess = new(1, 1);
        static void Main(string[] args)
        {
            //simulo il lavoro del barbiere -> il servente
            Thread barber = new Thread(Barber);
            barber.Start();
            for (int i = 0; i <NumberOfClientsInSim; i++)
            {
                new Thread(Client).Start();
                Thread.Sleep(500);
            }
            Console.WriteLine($"Fine: il barbiere ha fatto {clientiFatti} e ha perso  {clientiPersi}");
        }

        private static void Client(object? obj)
        {
            //il cliente guarda se c'è posto
            //se c'è posto si siede e attende di essere servit
            //se non c'è posto se ne va
            seatAccess.Wait();//apro la sezione critica
            if (freeSeats > 0)//c'è posto
            {
                //il cliente si siede
                freeSeats--;
                Console.WriteLine($"Il cliente con Thread Id = {Environment.CurrentManagedThreadId} si siede, ci sono ancora {freeSeats} posti liberi");
                seatAccess.Release();//chiudo la sezione critica
                clientReady.Release();
                //attendo che il barbiere si liberi
                barberReady.Wait();
                Console.WriteLine($"Il cliente con Thread Id = {Environment.CurrentManagedThreadId} sta per salire sulla poltrona di taglio");
                clientiFatti++;
            }
            else
            {
                seatAccess.Release();
                Console.WriteLine($"Il cliente con Thread ID = {Environment.CurrentManagedThreadId} non trova posto e se ne va");
                clientiPersi++;
            }
        }

        private static void Barber(object? obj)
        {
            while(true)
            {
                //rimane in attesa di un cliente
                Console.WriteLine("Il barbiere è in attesa di un nuovo cliente");
                clientReady.Wait();
                //c'è almeno un cliente, lo fa accomodare sulla poltrona di taglio
                seatAccess.Wait();
                freeSeats++; //si libera un posto nella sala d'attesa
                Console.WriteLine("Il barbiere libera un posto");
                seatAccess.Release();
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
