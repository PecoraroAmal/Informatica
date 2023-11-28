namespace TaskContinuation01
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task<int> t = Task.Run(() =>
            {
                return 32;
            }).ContinueWith((i) =>
            {
                return i.Result * 2;
            });
            t.ContinueWith((i) =>
            {
                Console.WriteLine(i.Result);
            });
            Console.WriteLine("Press Enter to terminate!");
            Console.ReadLine();
        }
    }
}
