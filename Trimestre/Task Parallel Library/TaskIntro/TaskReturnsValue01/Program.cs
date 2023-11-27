namespace TaskReturnsValue01
{
    class Program
    {
        static void Main(string[] args)
        {
            //qui int è il tipo restituito dal Task
            //si noti che in questo caso si è usato il costrutto Task.Run(lambda) e non new Task(lambda)
            Task<int> t = Task.Run(() =>
            {
                return 32;
            });
            //qui leggiamo il valore restituito dal Task
            //è come fare una Join sul thread del Task oppure come invocare la Wait sul Task
            Console.WriteLine(t.Result);
            Console.WriteLine();
            Console.WriteLine("Press Enter to terminate!");
            Console.ReadLine();
        }
    }
}