namespace CovarianceContravariance
{
    class Program

    {
        public delegate TextWriter CovarianceDel(); 
        public delegate void ContravarianceDel(StreamWriter streamWriter);

        static void Main()

        {
            CovarianceDel del;
            del = MethodStream;
            //l'assegnazione finziona perché il tipo restituito da MethodStream discende da TextWriter ciò si chiamba Covalenza
            del = MethodString;
            //l'assegnazione finziona perché il tipo restituito da MethodString discende da TextWriter ciò si chima Covalenza
            ContravarianceDel newdel = DoSomething;
            //l'assegnazione finziona perché il tipo dell'argomento del metodo che si chiama DoSomething è tale che StreamWriter discende da essociò si chiama Controvalenza
            Console.ReadLine();
            Console.WriteLine();
        }
        public static StreamWriter? MethodStream() { return null; }
        public static StringWriter? MethodString() { return null; }
        public static void DoSomething(TextWriter textWriter) { }
    }
}