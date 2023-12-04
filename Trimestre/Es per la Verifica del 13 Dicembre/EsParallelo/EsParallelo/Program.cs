
using System.ComponentModel.Design;
using System.Diagnostics;

namespace EsParallelo
{
    internal class Program
    {
        //Data la costante numerica N = 100 000 000, si considerino le seguenti attività di calcolo:

        //Determinazione e stampa della somma dei reciproci dei quadrati dei primi N numeri interi positivi:
        //SQ = 1/1^2 + 1/2^2 + 1/3^2 - ... + 1/N^2

        //Determinazione e stampa della somma dei logaritmi in base 10 dei primi N numeri interi positivi:
        //SL = log10(1) + log10(2) + log10(3) + ... + log10(N)

        //Determinazione e stampa della somma delle radici quadrate dei primi N numeri interi positivi:
        //SR = sqrt(1) + sqrt(2) + sqrt(3) + ... + sqrt(N)

        //Ogni attività termina con la stampa del tempo impiegato per ottenere il risultato (si utilizzino tre "cronometri" distinti).
        //a) Realizzare un'applicazione concorrente CalcoliJoinCount in C# (di tipo console) che esegua le tre
        //attività in parallelo implementando il costrutto join(count). In particolare il thread che esegue SQ
        //fa partire SL e poi procede al suo calcolo; il thread che esegue SL fa partire SR e poi esegue il suo
        //calcolo specifico.
        //Il main fa partire solo il thread che esegue SQ, ma deve attendere che tutti i thread abbiano finito
        const int N = 100_000_000;
        static CountdownEvent count = new CountdownEvent(3);
        static double Sq;
        static double Sl;
        static double Sr;
        static readonly Stopwatch sq = Stopwatch.StartNew();
        static readonly Stopwatch sl = Stopwatch.StartNew();
        static readonly Stopwatch sr = Stopwatch.StartNew();
        static readonly Stopwatch complessivo = Stopwatch.StartNew();
        static void Main(string[] args)
        {
            Thread SommaReciproci = new Thread(SQ);
            complessivo.Start();
            SommaReciproci.Start();
            count.Wait();
            complessivo.Stop();
            var complessivoMilli = complessivo.ElapsedMilliseconds;
            Console.WriteLine($"Tempo totale impiegato: {complessivoMilli}");
        }

        private static void SQ(object? obj)
        {
            Thread SommaLog = new Thread(SL);
            SommaLog.Start();
            sq.Start();
            for (long i = 1; i <= N+1; i++)
            {
                checked
                {
                    Sq += 1 / (i*i);
                }
            }
            sq.Stop();
            var sqMilli = sq.ElapsedMilliseconds;
            Console.WriteLine($"1) Somma dei reciproci dei quadrati dei primi {N} numeri interi positivi: {Sq} calcolato in {sqMilli}ms");
            count.Signal();
        }
        private static void SL(object? obj)
        {
            Thread SommaRadici = new Thread(SR);
            SommaRadici.Start();
            sr.Start();
            for (long i = 1; i <= N; i++)
            {
                checked
                {
                    Sl += Math.Log10(i);
                }
            }
            sr.Stop();
            var srMilli = sr.ElapsedMilliseconds;
            Console.WriteLine($"2) Somma dei logaritmi in base 10 dei primi {N} numeri interi positivi: {Sl} calcolato in {srMilli}ms");
            count.Signal();
        }
        private static void SR(object? obj)
        {
            sl.Start();
            for (long i = 0; i < N; i++)
            {
                checked
                {
                    Sr += Math.Sqrt(i);
                }
            }
            sl.Stop();
            var slMilli = sl.ElapsedMilliseconds;
            Console.WriteLine($"3) Somma delle radici quadrate dei primi {N} numeri interi positivi: {Sr} calcolato in {slMilli}ms");
            count.Signal();
        }
    }
}
