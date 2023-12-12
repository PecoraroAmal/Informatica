namespace Attached_DetachedTask
{
    internal class Program
    {
        public static void Main()
        {
            var parent = Task.Factory.StartNew(() => {
                Console.WriteLine("Outer task executing.");
                var child = Task.Factory.StartNew(() => {
                    Console.WriteLine("Nested task starting.");
                    Thread.SpinWait(500000);//tipo lo sleep ma misura i cicli macchina
                    Console.WriteLine("Nested task completing.");
                });
            });
            parent.Wait();
            Console.WriteLine("Outer has completed.");
        }
    }
}
