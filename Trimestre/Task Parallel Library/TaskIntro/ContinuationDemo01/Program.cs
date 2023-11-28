namespace ContinuationDemo01
{
    class Program
    {
        static void Main(string[] args)
        {
            // Execute the antecedent.
            Task<DayOfWeek> taskA = Task.Run(() => DateTime.Today.DayOfWeek);
            // Execute the continuation when the antecedent finishes.
            taskA.ContinueWith(antecedent => Console.WriteLine("Today is {0}.", antecedent.Result));
            Console.ReadLine();
        }
    }
}