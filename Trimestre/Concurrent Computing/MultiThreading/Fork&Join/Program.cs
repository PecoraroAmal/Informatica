namespace Fork_Join
{
    class Program
    {
        static int A, B, C, D, E, F, G, H;
        static void Main(string[] args)
        {
            Thread t1 = new Thread(Fork1);
            Thread t2 = new Thread(Fork2);
            t1.Start();
            t2.Start();
            A = 10;
            D = A + 5;
            Console.WriteLine("D: {0}", D);
            t1.Join(); // Il main thread attende prima la fine di t1...
            G = E - D;
            Console.WriteLine("G: {0}", G);
            t2.Join(); // ... e poi quella di t2.
            H = F + G;
            Console.WriteLine("H: {0}", H);
        }
        static void Fork1()
        {
            B = 20;
            E = B * 2;
            Console.WriteLine("E: {0}", E);
        }
        static void Fork2()
        {
            C = 30;
            F = C * C;
            Console.WriteLine("F: {0}", F);
        }
    }
}
