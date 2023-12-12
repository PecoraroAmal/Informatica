using System.Diagnostics;
using System.Reflection.Metadata;

namespace TaskConParametri
{
    //Voglio passare dei parametri all'attività del task
    //Voglio che ogni task mi restituisca dei dati
    class MyData
    {
        public double Somma { get; set; }
        public long Millisecondi { get; set; }
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

            const int N = 100_000_000;
            static readonly Stopwatch complessivo = Stopwatch.StartNew();
            static void Main(string[] args)
            {
                //Creazione
                complessivo.Start();
                Task<MyData> taskSq = Task.Factory.StartNew(() => SQ(N));
                Task<MyData> taskSl = Task.Factory.StartNew(() => SL(N));
                Task<MyData> taskSr = Task.Factory.StartNew(() => SR(N));
                //Attendo la fine di tutti i task
                Task.WaitAll(taskSq, taskSl, taskSr);
                complessivo.Stop();
                //Per accedere al valore restituito da un task possiamo usare la property Result
                Console.WriteLine($"La somma dei reciproci dei quadrati è {taskSq.Result.Somma} e ha impiegato {taskSq.Result.Millisecondi}");
                Console.WriteLine($"La somma dei logaritmi è {taskSl.Result.Somma} e ha impiegato {taskSl.Result.Millisecondi}");
                Console.WriteLine($"La somma dele raidi è {taskSr.Result.Somma} e ha impiegato {taskSr.Result.Millisecondi}");
                var complessivoMilli = complessivo.ElapsedMilliseconds;
                Console.WriteLine($"Tempo totale impiegato: {complessivoMilli}");
            }

            static MyData SQ(int n)
            {
                Stopwatch sq = Stopwatch.StartNew();
                double Sq = 0;
                for (long i = 1; i <= N + 1; i++)
                {
                    checked
                    {
                        Sq += 1.0 / (i * i);
                    }
                }
                sq.Stop();
                MyData myData = new MyData() { Somma = Sq, Millisecondi = sq.ElapsedMilliseconds };
                return myData;
            }
            static MyData SL(int n)
            {
                Stopwatch sl = Stopwatch.StartNew();
                double Sl = 0;
                for (long i = 1; i <= N; i++)
                {
                    checked
                    {
                        Sl += Math.Log10(i);
                    }
                }
                sl.Stop();
                MyData myData = new MyData() { Somma = Sl, Millisecondi = sl.ElapsedMilliseconds };
                return myData;
            }
            static MyData SR(int n)
            {
                Stopwatch sr = Stopwatch.StartNew();
                double Sr = 0;
                for (long i = 0; i < N; i++)
                {
                    checked
                    {
                        Sr += Math.Sqrt(i);
                    }
                }
                sr.Stop();
                MyData myData = new MyData() { Somma = Sr, Millisecondi = sr.ElapsedMilliseconds };
                return myData;
            }
        }
    }
}
