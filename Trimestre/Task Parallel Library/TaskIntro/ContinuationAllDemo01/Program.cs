namespace ContinuationAllDemo01
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Task<int>> tasks = new List<Task<int>>();
            for (int ctr = 1; ctr <= 10; ctr++)
            {
                int baseValue = ctr;
                tasks.Add(Task.Factory.StartNew((b) => {
                    int i = (int)b;
                    return i * i;
                }, baseValue));
            }
            //continuation è di tipo Task<int[]>, restituisce un array di valori dove ogni valore è quello restituito da ciascun task lanciato
            var continuation = Task.WhenAll(tasks);
            long sum = 0;
            for (int ctr = 0; ctr <= continuation.Result.Length - 1; ctr++)
            {
                Console.Write($"{continuation.Result[ctr]}{(ctr == continuation.Result.Length - 1 ? " = " : " + ")}");
                sum += continuation.Result[ctr];
            }
            Console.WriteLine(sum);
        }
    }
}