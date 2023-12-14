using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SommaNumeriPrimi
{
    class Program
    {
        static int Max = 100_000;
        static int n = 3;
        static int sommaSequenziale = 0;
        static int sommaParallela = 0;
        static int sommaThread = 0;

        static void Main()
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            sommaSequenziale = Sequenziale(n);
            stopwatch.Stop();
            var seq = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Somma sequenziale dei primi {Max} numeri primi: {sommaSequenziale}");
            Console.WriteLine($"Tempo impiegato in modo sequenziale: {stopwatch.ElapsedMilliseconds} ms");

            stopwatch.Restart();
            sommaParallela = Parallela(n);
            stopwatch.Stop();
            var par = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Somma parallela dei primi {Max} numeri primi: {sommaParallela}");
            Console.WriteLine($"Tempo impiegato in modo parallelo: {stopwatch.ElapsedMilliseconds} ms");

            stopwatch.Restart();
            sommaThread = ThreadLocalData(n);
            stopwatch.Stop();
            var thr = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Somma con dati locali ai thread dei primi {Max} numeri primi: {sommaThread}");
            Console.WriteLine($"Tempo impiegato con dati locali ai thread: {stopwatch.ElapsedMilliseconds} ms");

            // Calcolo del fattore di speedup tra sequenziale e parallelo
            double speedupParallelo = (double)seq / par;
            Console.WriteLine($"Fattore di speedup tra sequenziale e parallelo: {speedupParallelo}");

            // Calcolo del fattore di speedup tra sequenziale e ThreadLocalData
            double speedupThreadLocalData = (double)seq / thr;
            Console.WriteLine($"Fattore di speedup tra sequenziale e ThreadLocalData: {speedupThreadLocalData}");
        }

        private static int ThreadLocalData(int n)
        {
            int result = 0;
            object lockObj = new object();

            Parallel.For(2, Max, () => 0, (i, state, local) =>
            {
                if (Primo(i))
                {
                    local += i;
                }
                return local;
            },
            local => Interlocked.Add(ref result, local));

            return result;
        }

        private static int Parallela(int n)
        {
            int result = 0;
            object lockObj = new object();

            Parallel.For(2, Max, i =>
            {
                if (Primo(i))
                {
                    lock (lockObj)
                    {
                        result += i;
                    }
                }
            });

            return result;
        }

        private static int Sequenziale(int n)
        {
            int result = 0;
            for (int i = 2; i < Max; i++)
            {
                if (Primo(i))
                {
                    result += i;
                }
            }
            return result;
        }

        private static bool Primo(int num)
        {
            if (num < 2) return false;
            for (int i = 2; i <= Math.Sqrt(num); i++)
            {
                if (num % i == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
