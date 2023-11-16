using System.Threading;

namespace ProducerConsumerFIFO
{
    class Program
    {
        static readonly object _lock = new();
        static int bufferSize = 10;
        static int[] buffer = new int[bufferSize];
        static SemaphoreSlim emptyPosCount = new(bufferSize, bufferSize);
        static SemaphoreSlim fillPosCount = new(0, bufferSize);
        static int writePos = 0;
        static int readPos = 0;
        static void Main(string[] args)
        {
            Thread producer = new(ProducerWork) { Name = "Producer" };
            Thread consumer = new(ConsumerWork) { Name = "Consumer" };
            producer.Start();
            consumer.Start();
            //attendo la fine dei thread producer e consumer
            producer.Join();
            consumer.Join();
            Console.WriteLine("Fine");
        }
        private static void ConsumerWork()
        {
            while (true)
            {
                fillPosCount.Wait();//passo il semaforo - ci sono celle piene
                lock (_lock)
                {
                    buffer[readPos] = 0;
                    Console.WriteLine("Consumato prodotto alla posizione {0} da thread id = { 1}, thread name = { 2 }",readPos,
                        Environment.CurrentManagedThreadId,Thread.CurrentThread.Name);
                    Console.WriteLine("Numero celle libere {0}",
                   emptyPosCount.CurrentCount + 1);
                    PrintArray(buffer);
                //incremento modulo BufferSize - coda circolare
 readPos = (readPos + 1) % bufferSize;
                }
                emptyPosCount.Release();
                Thread.Sleep(2500);
            }
        }
        private static void ProducerWork()
        {
            while (true)
            {
                emptyPosCount.Wait();//passo il semaforo - ci sono celle vuote
                lock (_lock)//sezione critica
                {
                    buffer[writePos] = 1;
                    Console.WriteLine("Aggiunto prodotto alla posizione {0} da thread id = { 1}, thread name = { 2 }",writePos,
                        Environment.CurrentManagedThreadId, Thread.CurrentThread.Name);
                    Console.WriteLine("Numero celle occupate {0}",
                   fillPosCount.CurrentCount + 1);
                    PrintArray(buffer);
                    //incremento modulo BufferSize - coda circolare
                    writePos = (writePos + 1) % bufferSize;
                }
                fillPosCount.Release();
                Thread.Sleep(1000);
            }
        }
        static void PrintArray(int[] array)
        {
            foreach (var item in array)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine();
        }
    }
}