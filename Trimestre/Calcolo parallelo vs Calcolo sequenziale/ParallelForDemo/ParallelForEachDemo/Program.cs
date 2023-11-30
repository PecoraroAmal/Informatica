using System.Collections.Concurrent;
using System.Diagnostics;

namespace ParallelForEachDemo
{
    class Program
    {
        //https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-write-a-simpleparallel-foreach-loop
        static void Main()
        {
            // 2 million
            var limit = 30_000_000;//l'underscore non inficia sul programma, serve per vedere meglio le cifre
            var numbers = Enumerable.Range(0, limit).ToList();//creo una lista con i valori da 0 a limit
            var watch = Stopwatch.StartNew();
            var primeNumbersFromForeach = GetPrimeList(numbers);//restituisco la lista con i numeri primi
            watch.Stop();
            var watchForParallel = Stopwatch.StartNew();
            var primeNumbersFromParallelForeach = GetPrimeListWithParallel(numbers);
            //restituisco la lista con i numeri primi con il calcolo parallelo
            watchForParallel.Stop();
            Console.WriteLine($"Classical Foreach loop | Total prime numbers : {primeNumbersFromForeach.Count} | Time Taken: {watch.ElapsedMilliseconds} ms.");
            Console.WriteLine($"Parallel  ForEach loop | Total prime numbers : {primeNumbersFromParallelForeach.Count} | Time Taken: {watchForParallel.ElapsedMilliseconds} ms.");
            //calcoliamo il fattore di speedup
            double speedup = (double)(watch.ElapsedMilliseconds) / watchForParallel.ElapsedMilliseconds;
            Console.WriteLine($"Il fattore di speedup è {speedup:F2}. Dunque Il calcolo parallelo è {speedup:F2} volte più veloce di quello sequenziale");
        }
        /// <summary>
        /// GetPrimeList returns Prime numbers by using sequential ForEach
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        private static IList<int> GetPrimeList(IList<int> numbers) => numbers.Where(IsPrime).ToList();
        //filtro i numeri contenuti nella lista in base al metodo/predicatoIsPrime
        /// <summary>
        /// GetPrimeListWithParallel returns Prime numbers by using Parallel.ForEach
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        private static IList<int> GetPrimeListWithParallel(IList<int> numbers)
        {
            //https://learn.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentbag1?view=net-7.0
            //Represents a thread-safe, unordered collection of objects.
            var primeNumbers = new ConcurrentBag<int>();
            Parallel.ForEach(numbers, number =>
            {
                if (IsPrime(number))
                {
                    primeNumbers.Add(number);
                }
            });
            return primeNumbers.ToList();
        }
        /// <summary>
        /// IsPrime returns true if number is Prime, else false.(https://en.wikipedia.org/wiki/Prime_number)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static bool IsPrime(int number)
        {
            if (number <= 2)
            {
                return false;
            }
            for (var divisor = 2; divisor <= Math.Sqrt(number); divisor++)
            {
                if (number % divisor == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }

}
