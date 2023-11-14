namespace SleepingDemo01
{
    class Program
    {
        static void Main(string[] args)
        {
            // Interrupt a sleeping thread. 
            var sleepingThread = new Thread(SleepIndefinitely);
            sleepingThread.Name = "Sleeping";
            sleepingThread.Start();
            Thread.Sleep(2000);
            sleepingThread.Interrupt();

            //Abort è deprecato nelle ultime versioni di .NET

        }
        private static void SleepIndefinitely()
        {
            Console.WriteLine("Thread '{0}' about to sleep indefinitely.",Thread.CurrentThread.Name);
            try
            {
                Thread.Sleep(Timeout.Infinite);
            }
            catch (ThreadInterruptedException)
            {
                Console.WriteLine("Thread '{0}' awoken.",Thread.CurrentThread.Name);
            }
            finally
            {
                Console.WriteLine("Thread '{0}' executing finally block.",Thread.CurrentThread.Name);
            }
            Console.WriteLine("Thread '{0}' finishing normal execution.",Thread.CurrentThread.Name);
            Console.WriteLine();
        }
    }
}