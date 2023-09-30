namespace ActionDemo
{
    class Program
    {
        public static void Main()
        {
            //Action è un costrutto di .NET che permette di definire delegati che accettano parametri del tipo specificato tra <> e che restituiscono void
            Action<int, int> mathDelegate = (x, y) =>
            {
                Console.WriteLine(x + y);
            };
            mathDelegate(5, 2);
            // output: 7
            mathDelegate = (x, y) => Console.WriteLine(x - y);
            mathDelegate(5, 2);
            // output: 3
            Console.ReadLine();
        }
    }
}