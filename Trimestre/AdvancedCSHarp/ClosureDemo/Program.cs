namespace ClosureDemo
{
    internal class Program
    {
        delegate void MyAction();
        static MyAction GetAction()
        {
            int x = 0;
            MyAction a = delegate { Console.WriteLine(x); };
            x = 1;
            return a;
        }

        static void Main(string[] args)
        {
            MyAction a = GetAction();
            a();
            Console.ReadLine();
        }
    }
}