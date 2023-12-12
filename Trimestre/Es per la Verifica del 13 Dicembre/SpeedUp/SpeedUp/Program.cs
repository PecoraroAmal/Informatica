using System.Diagnostics;

namespace SpeedUp
{
    internal class Program
    {
        //Usare le potenzialità del calcolo parallelo per migliorare le prestazioni del seguente calcolo.
        //const long numOfElements = 100000000;
        //double[] sum = new double[numOfElements];
        //    for (int i = 0; i<numOfElements; i++)
        //    {
        //        sum[i] = Math.Cos(i) + Math.Sin(i);
        //    }
        //Valutare il fattore di speedup raggiunto con il calcolo parallelo
        static void Main(string[] args)
        {
            const long numOfElements = 100000000;
            double[] sum = new double[numOfElements];
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < numOfElements; i++)
            {
                sum[i] = Math.Cos(i) + Math.Sin(i);
            }
            sw.Stop();
            long tempoSequenziale = sw.ElapsedMilliseconds;
            Console.WriteLine($"Tempo impiegrato per generare i valori in sequenza {tempoSequenziale}ms");
            sw.Restart();
            Parallel.For(0, numOfElements, (i) =>
            {
                sum[i] = Math.Cos(i) + Math.Sin(i);
            });
            sw.Stop();
            long tempoParallelo = sw.ElapsedMilliseconds;
            Console.WriteLine($"Tempo impiegrato per generare i valori in parallelo {tempoParallelo}ms");
            double speedup = (double)tempoSequenziale / tempoParallelo;
            Console.WriteLine($"Fattore di speedup è: {speedup:F3}");
        }
    }
}
