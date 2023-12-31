﻿using System.Runtime.ConstrainedExecution;

namespace Es1
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

        //Dati condivisi
        const int MaxBimbi = 50;
        const int postiliberi = 5;
        static SemaphoreSlim PostiLiberi = new (postiliberi, postiliberi);
        static Random random = new Random();
        static readonly object _lock = new object();
        static void Main(string[] args)
        {
            //simuliamo l'avento dei bambini
            for(int i = 0; i < MaxBimbi; i++)
            {
                new Thread(Giostra).Start(i);
                Thread.Sleep(100);
            }
        }

        private static void Giostra(object? obj)
        {
            int indice = obj == null ? -1 : (int)obj;
            Console.WriteLine($"Sono il bambino {indice}-mo, con Thread Id = {Environment.CurrentManagedThreadId} e attendo di salire sulla giostra");
            PostiLiberi.Wait();
            Console.WriteLine($"Sono il bambino {indice}-mo, con Thread Id = {Environment.CurrentManagedThreadId} e sono sulla giostra");
            //siccome più thread potrebbero lanciare il metodo Next contemporaneamente potremmo avere interferenze, pertanto usaimo il lock
            int randomMillisecond;
            lock(_lock)
            {
                randomMillisecond = random.Next(1000, 3001);
            }
            Thread.Sleep(randomMillisecond);
            Console.WriteLine($"Sono il bambino {indice}-mo, con Thread Id = {Environment.CurrentManagedThreadId} e scendo dalla giostra");
            PostiLiberi.Release();
        }
    }
}
