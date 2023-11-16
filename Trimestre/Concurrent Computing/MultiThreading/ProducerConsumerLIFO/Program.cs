using System.Threading;

namespace ProducerConsumerLIFO
{
    class Program
    {
        static readonly object _lock = new();
        static int BufferSize = 10;
        static SemaphoreSlim emptyPosCount = new(BufferSize, BufferSize);
        static SemaphoreSlim fillPosCount = new(0, BufferSize);
        static int firstEmptyPos = 0;
        static int[] buffer = new int[BufferSize];
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
                    buffer[--firstEmptyPos] = 0;//simulo la lettura del dato
                    Console.WriteLine("Consumato prodotto alla posizione {0} da thread id = { 1}, thread name = { 2 }",firstEmptyPos,
                        Environment.CurrentManagedThreadId,Thread.CurrentThread.Name);
                    PrintArray(buffer);
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
                    buffer[firstEmptyPos] = 1;
                    Console.WriteLine("Aggiunto prodotto alla posizione {0} da thread id = { 1}, thread name = { 2 }",firstEmptyPos,
                        Environment.CurrentManagedThreadId,Thread.CurrentThread.Name);
                    PrintArray(buffer);
                firstEmptyPos++;
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
