namespace ProduttoreConsumatoreFIFO
{
    internal class Program
    {
        static readonly object _lock = new object();
        const int BufferSize = 10;
        static int[] buffer = new int[BufferSize];
        static SemaphoreSlim emptyPosCount = new SemaphoreSlim(BufferSize, BufferSize);//conta le celle libere
        static SemaphoreSlim fillPosCount = new SemaphoreSlim(0, BufferSize);//conta le celle occupate
        static int writePos = 0;
        static int readPos = 0;
        static void Main(string[] args)
        {
            Thread produttore = new Thread(Produttore) { Name = "Produttore" };
            Thread consumatore = new Thread(Consumatore) { Name = "Consumatore" };
            //faccio partire i due thread
            produttore.Start();
            consumatore.Start();
            //il main thread aspetta la fine dei thread
            produttore.Join();
            consumatore.Join();
        }

        private static void Consumatore(object? obj)
        {
            while (true)
            {
                fillPosCount.Wait();
                //ci sono celle piene
                //consumo una cella
                lock (_lock)
                {
                    buffer[readPos] = 0;
                    Console.WriteLine($"Consumato elemento in posizione {readPos} dal Thread con id: {Environment.CurrentManagedThreadId} con nome: {Thread.CurrentThread.Name}");
                    readPos = (readPos + 1) % BufferSize;
                }
                //incremento il numero delle celle vuote
                PrintArray(buffer);
                emptyPosCount.Release();
                Thread.Sleep(1000);
            }
        }

        private static void Produttore(object? obj)
        {
            while (true)
            {
                emptyPosCount.Wait();
                //ci sono celle vuote
                //il thread odvrebbe uscire il prima possbile dalla sezione critica
                lock (_lock)
                {
                    buffer[writePos] = 1;
                    Console.WriteLine($"Prodotto elemento in posizione {writePos} dal Thread con id: {Environment.CurrentManagedThreadId} con nome: {Thread.CurrentThread.Name}");
                    writePos = (writePos + 1) % BufferSize;
                }
                PrintArray(buffer);
                fillPosCount.Release();
                Thread.Sleep(1000);
            }
        }
        private static void PrintArray(int[] buffer)
        {
            foreach (var item in buffer)
            {
                Console.Write(item + " | ");
            }
            Console.WriteLine();
        }
    }
}
