namespace Lambda
{
    class Program

    {
        public delegate double MathDelegate(double value1, double value2);
        public static void Main()
        {
            MathDelegate mathDelegate = (x, y) => x + y;
            var result = mathDelegate(5, 2);
            Console.WriteLine(result);
            // output: 7
            mathDelegate = (x, y) => x - y;
            result = mathDelegate(5, 2);
            Console.WriteLine(result);
            // output: 3
            Console.ReadLine();
        }
    }
}