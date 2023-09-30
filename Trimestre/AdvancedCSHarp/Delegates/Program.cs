using System.Text;

namespace Delegates
{
    class Program
    {
        // declare delegate type
        // dichiaro il tipo delegato che si chiama Print
        public delegate void Print(int value);
        public delegate double MathDelegate(double value1, double value2);
        static void Main(string[] args)
        {
            Console.WriteLine("----------------\nPrimo esempio\n----------------");
            // Print delegate points to PrintNumber
            //dichiaro una variabile di tipo print chepunta al codice del mettodo PrintNumber
            Print printDel = PrintNumber;
            // or
            // Print printDel = new Print(PrintNumber);
            printDel(100000); // è come se stessi facendo: PrintNumber(100000)
            printDel(200);
            // Print delegate points to PrintMoney
            Console.OutputEncoding = Encoding.UTF8; //per far apparire il carattere €
            printDel = PrintMoney;
            printDel(10000);
            printDel(200);
            //per invocare il metodo invocato dal delegato si può usare anche la sisntassi seguent:
            printDel.Invoke(200);
            //secondo esempio
            Console.WriteLine("----------------\nSecondo esempio\n----------------");
            MathDelegate mathDelegate = Add;
            var result = mathDelegate(5, 2);
            Console.WriteLine($"Il risultato è: {result}");
            // output: 7
            mathDelegate = Subtract;
            result = mathDelegate(5, 2);
            Console.WriteLine($"Il risultato è: {result}");
            // output: 3
            //mathDelegate = SommaIntera(5, 7); int non discende da double quindi non posso farlo
            Console.ReadLine();
        }

        public static void PrintNumber(int num)
        {
            Console.WriteLine("Number: {0,-12:N0}", num);
        }

        public static void PrintMoney(int money)
        {
            Console.WriteLine("Money: {0:C}", money);
        }
        public static double Add(double value1, double value2)
        {
            return value1 + value2;
        }

        public static double Subtract(double value1, double value2)
        {
            return value1 - value2;
        }
        public static int SommaIntera(double value1, double value2)
        {
            return (int)(value1 +value2);
        }
    }
}