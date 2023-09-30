namespace DelegatesMulticast
{
    class Program
    {
        delegate void Del(string s);
        static void Main()
        {
            Del a, b, c, d, k;
            // Create the delegate object a that references
            // the method Hello:
            a = new Del(Hello);
            // Create the delegate object b that references
            // the method Goodbye:
            b = Goodbye;
            // The two delegates, a and b, are composed to form c:
            c = a + b; //c punta sia ad a che b, quindi è una lista di puntatori
            //allo stesso modo possiamo inizializzare una variabile delegate Del a null
            //e usare += per aggiungere un metodo
            k = null;
            //aggiungo un metodo
            k += a;
            //aggiungo un altro metodo
            k += b;
            // Remove a from the composed delegate, leaving d,
            // which calls only the method Goodbye:
            d = c - a; //sarebbe a+b-a quindi b
            Console.WriteLine("Invoking delegate a:");
            a("A");
            Console.WriteLine("Invoking delegate b:");
            b("B");
            Console.WriteLine("Invoking delegate c:");
            c("C");
            Console.WriteLine("Invoking delegate d:");
            d("D");
            Console.WriteLine("Invoking delegate k:");
            k("K");
            // Output:
            //Invoking delegate a:
            //Hello, A!
            //Invoking delegate b:
            //Goodbye, B!
            //Invoking delegate c:
            //Hello, C!
            //Goodbye, C!
            //Invoking delegate d:
            //Goodbye, D!
            //Invoking delegate k:
            //Hello, K!
            //Goodbye, K!
            int invocationCount = k.GetInvocationList().GetLength(0);
            Console.WriteLine($"I delegati associati a k sono: {invocationCount}");
            Console.ReadLine();
        }
        static void Hello(string s)
        {
            Console.WriteLine("Hello, {0}!", s);
        }

        static void Goodbye(string s)
        {
            Console.WriteLine("Goodbye, {0}!", s);
        }
    }

}