namespace JoinCountDemo
{
    class Program
    {
        static int A, B, C, D, E, F, Z;
        static CountdownEvent count = new CountdownEvent(3);
        static void Main(string[] args)
        {
            A = 10;
            Thread t = new Thread(Fork1);
            t.Start();
            D = A * 4;
            Console.WriteLine("D: {0}", D);
            count.Signal(); // Decrementa il contatore
            count.Wait(); // Attende che il contatore sia uguale a zero
            Z = D + E + F;
            Console.WriteLine("Z: {0}", Z);
        }
        static void Fork1()
        {
            Console.WriteLine("Procedura n. 1");
            B = A + 20;
            Thread t = new Thread(Fork2);
            t.Start();
            E = B - 5;
            Console.WriteLine("E: {0}", E);
            count.Signal(); // Decrementa il contatore
        }
        static void Fork2()
        {
            Console.WriteLine("Procedura n. 2");
            C = A + B;
            F = C + 6;
            Console.WriteLine("F: {0}", F);
            count.Signal(); // Decrementa il contatore
        }
    }
}