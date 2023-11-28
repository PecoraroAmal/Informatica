namespace DifferentContinuationTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<int> t = Task.Run(() =>
            {
                //se il numero è messo a zero viene sollevata un'eccezione non gestita
                int numero = 1;
                if (numero == 0)
                {
                    throw new Exception();
                }
                return 32;
            });
            t.ContinueWith((i) =>
            {
                Console.WriteLine("Faulted");
            }, TaskContinuationOptions.OnlyOnFaulted);
            t.ContinueWith((i) =>
            {
                Console.WriteLine("Canceled");
            }, TaskContinuationOptions.OnlyOnCanceled);
            var completedTask = t.ContinueWith((i) =>
            {
                Console.WriteLine(i.Result);
                Console.WriteLine("Completed");
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
            Console.WriteLine("Press Enter to terminate!");
            Console.ReadLine();
        }
    }
}
