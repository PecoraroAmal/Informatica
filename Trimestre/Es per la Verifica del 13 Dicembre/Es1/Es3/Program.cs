
using System.Data;

namespace Es3
{
    internal class Program
    {
        //esercizio ABBC ABBC, 
        //Crea e fai partire 3 Thread, che stampano sulla console rispettivamente le lettere A, B e C.
        //Noterai che l’ordine di esecuzione dei thread è casuale, e di conseguenza sulla console vedrai stampato una sequenza
        //casuale di lettere. Scrivi un programma che sincronizzi l’esecuzione dei thread in modo tale che sulla console venga
        //stampato in modo deterministico la sequenza: ABBC ABBC ABBC ABBC ABBC ABBC …
        static SemaphoreSlim goA = new SemaphoreSlim(1,1);
        static SemaphoreSlim goB = new SemaphoreSlim(0,2);
        static SemaphoreSlim goC = new SemaphoreSlim(0,1);
        static void Main(string[] args)
        {
            Thread a = new Thread(ScriviA);
            Thread b = new Thread(ScriviB);
            Thread c = new Thread(ScriviC);
            a.Start();
            b.Start();
            c.Start();
        }

        private static void ScriviA(object? obj)
        {
            while (true)
            {
                goA.Wait();
                Console.Write("A");
                Thread.Sleep(100);
                goB.Release(2); //è uguale a fare 2 volte la release
            }
        }

        private static void ScriviB(object? obj)
        {
            while (true)
            {
                goB.Wait();
                Console.Write("B");
                Thread.Sleep(100);
                if(goB.CurrentCount == 0)
                    goC.Release();
            }
        }

        private static void ScriviC(object? obj)
        {
            while(true)
            {
                goC.Wait();
                Console.Write("C ");
                Thread.Sleep(100);
                goA.Release();
            }
        }
    }
}
