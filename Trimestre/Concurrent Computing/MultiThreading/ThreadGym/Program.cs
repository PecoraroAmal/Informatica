using System;
using System.Threading;
using System.Threading.Tasks;
namespace ThreadGym
{
    class Program
    {
        private static SemaphoreSlim semaphore = null!;
        // A padding interval to make the output more orderly.
        private static int padding;
        public static void Main()
        {
            // Create the semaphore.
            semaphore = new SemaphoreSlim(0, 3);
            Console.WriteLine("{0} Thread can enter the semaphore.",
            semaphore.CurrentCount);
            Thread[] threads = new Thread[5];
            // Create and start five numbered tasks.
            for (int i = 0; i <= 4; i++)
            {
                threads[i] = new Thread(() => {
                    // Each task begins by requesting the semaphore.
                    Console.WriteLine("Thread {0} begins and waits for the semaphore.",Thread.CurrentThread.ManagedThreadId);
                    //blocca il thread/task corrente in attesa di entrare nel semaforo
                    semaphore.Wait();
                    //https://docs.microsoft.com/enus/dotnet/api/system.threading.interlocked?view=netframework-4.8
                    //Adds two integers and replaces the first integer with the sum, as an atomic operation.
                    Interlocked.Add(ref padding, 100);
                    Console.WriteLine("Thread {0} enters the semaphore.",Thread.CurrentThread.ManagedThreadId);
                    // The task just sleeps for 1+ seconds.
                    Thread.Sleep(1000 + padding);
                    //rilascio il semaforo e incrementa il contatore
                    Console.WriteLine("Thread {0} releases the semaphore; previous count:{ 1}.", 
                        Thread.CurrentThread.ManagedThreadId, semaphore.Release());
                });
            }
            foreach (var thread in threads)
            {
                thread.Start();
            }
            // Wait for half a second, to allow all the tasks to start and block.
            Thread.Sleep(500);
            // Restore the semaphore count to its maximum value.
            Console.Write("Main thread calls Release(3) --> ");
            //rilascia il semaforo del numero specificato di valori
            semaphore.Release(3);
            Console.WriteLine("{0} Threads can enter the semaphore.",
            semaphore.CurrentCount);
            // Main thread waits for the tasks to complete.
            foreach (var thread in threads)
            {
                thread.Join();
            }
            Console.WriteLine("Main thread exits.");
            Console.ReadLine();
        }
    }
}