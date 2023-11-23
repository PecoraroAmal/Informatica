using System;
using System.Threading;
namespace BarbiereSbaglaito
{
    class Program
    {
        //per la gestione della sezione critica si utilizza un Monitor (lock)
        const int numberOfSeats = 10;
        static int freeSeats = numberOfSeats;
        private static readonly object _lock = new object();
        static SemaphoreSlim barberReady = new SemaphoreSlim(1, 1);
        static SemaphoreSlim clientReady = new SemaphoreSlim(0, numberOfSeats);
        static void Main(string[] args)
        {
            //Attivazione del thread del barbiere il barbiere dorme sulla sua sedia da lavoro
            //quando arriva un cliente, il barbiere si sveglia e gli taglia i capelli
            //quando ha finito di tagliare i capelli, se c'è un altro cliente seduto
            //su una delle sedie del negozio, lo serve immediatamente, altrimenti ritorna a dormine
            //il barbiere si sveglia appena arriva un nuovo cliente
            Thread barber = new Thread(Barber);
            barber.Start();
            //Attivazione dei thread dei clienti
            //un cliente arriva in un momento qualsiasi, se trova un posto libero nella
            //sala d'attesa si siede e aspetta fino a che non viene servito; se arriva e trova
            //tutte le sedie occupate, se ne va via
            int numberOfClients = 30;
            for (int i = 0; i < numberOfClients; i++)
            {
                new Thread(Client).Start();
                //i clienti arrivano con una certa frequenza al negozio
                Thread.Sleep(500);
            }
            barber.Join();
        }
        private static void Client(object obj)
        {
            //il cliente deve verificare se c'è posto
            //per farlo deve controllare il numero di posti liberi e quindi deve accedere alla
            //sezione critica
            //QUESTA VERSIONE VA IN DEADLOCK - QUESTO E' VOLUTO PER MOSTRARE IL PROBLEMA DELL'ATTESA CIRCOLARE
            //IL DEADLOCK E' DOVUTO AL FATTO CHE QUANDO IL CLIENT PRENDE IL LOCK RIMANE IN ATTESA CHE IL BARBIERE SIA DISPONIBILE
            //MA IL BARBIERE PER TAGLIARE I CAPELLI DEVE PRENDERE IL LOCK CHE NON ARRIVERA' MAI
            lock (_lock)
            {
                if (freeSeats > 0)
                {
                    //il cliente si siede ed occupa un posto
                    freeSeats--;
                    Console.WriteLine($"Il cliente con Thread Id = {Thread.CurrentThread.ManagedThreadId} trova posto e attende di essere servito; " +
                $" posti disponibili = {freeSeats}");
                    clientReady.Release();//aumenta il numero di clienti disponibili
                    barberReady.Wait();//attende che il barbiere sia disponibile
                                       //il cliente viene servito
                    Console.WriteLine($"Sono il cliente con ThreadId = {Thread.CurrentThread.ManagedThreadId} " +
                $". Il Barbiere mi sta tagliando i capelli. Posti disponibili = {freeSeats}");
                }
                else
                {
                    //il cliente se ne va
                    Console.WriteLine($"Il cliente con ThreadId = {Thread.CurrentThread.ManagedThreadId} non ha trovato posto e se ne va");
                }
            }
        }
        private static void Barber(object obj)
        {
            while (true)
            {
                clientReady.Wait();//attendo che ci sia un cliente
                                   //il cliente si accomoda sulla sedia del barbiere e libera un posto
                Console.WriteLine("Barbiere: Sono in attesa di entrare in sezione critica");
                lock (_lock)
                {
                    freeSeats++;//il barbiere libera un posto
                    Console.WriteLine("Il barbiere libera un posto");
                }//il barbiere deve accedere alla sezione critica
                Console.WriteLine($"Il Barbiere sta tagliando i capelli");
                Thread.Sleep(2000);//il barbiere sta tagliando in capelli; questo è il tempo impiegato per tagliare i capelli
                barberReady.Release(); //il barbiere è nuovamente disponibile
            }
        }
    }
}