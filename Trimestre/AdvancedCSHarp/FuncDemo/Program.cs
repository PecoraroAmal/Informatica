namespace FuncDemo
{
    class Program
    {
        public static void Main()
        {
            //modalità alternativa per definire un delegato
            //Func è un costrutto speciale di .NET che permette di definire velocemente delegat: tra parentesi angoleate metto prima il tipo dei parametri di input, l'ultimo è 
            //il tipo restituito dal metodo
            Func<int, int, int> mathDelegate = (x, y) =>
            {
                Console.WriteLine("Add");
                return x + y;
            };
            var result = mathDelegate(5, 2);
            Console.WriteLine(result);
            // output: 7
            mathDelegate = (x, y) => x - y; ;
            result = mathDelegate(5, 2);
            Console.WriteLine(result);
            // output: 3
            Console.ReadLine();
        }
    }
}