using System.Diagnostics;

namespace ParallelForWithThreadLocalVariabiles
{
    internal class Program
    {
        static readonly object _lock = new object();
        static void Main(string[] args)
        {
            const int Numbers = 10_000_000;
            int[] nums = Enumerable.Range(1, Numbers).ToArray();//creo un array con i valori da 1 a N
            long total = 0;
            var watch = Stopwatch.StartNew();
            //Formula di Gauss con il Parallel.For non ottimizzata
            Parallel.For(0, Numbers, i =>
            {
                lock (_lock)
                {
                    total += nums[i];
                }
            });
            watch.Stop();
            var millisecondiLock = watch.ElapsedMilliseconds;
            Console.WriteLine($"La somma dei primi {Numbers} è: {total} e con il lock impiego: {millisecondiLock}ms");
            total = 0;
            watch.Restart();
            for(int i = 0; i < Numbers; i++)
            {
                total += nums[i];
            }
            watch.Stop();
            var millisecondiSequenziale = watch.ElapsedMilliseconds;
            Console.WriteLine($"La somma dei primi {Numbers} è: {total} e con il calcolo sequenziale impiego: {millisecondiSequenziale}ms");
            double speed = (double)millisecondiLock / millisecondiSequenziale;
            Console.WriteLine($"Speed: {speed}");
            total = 0;
            watch.Restart();
            Parallel.For(0, Numbers, i =>
            {
                Interlocked.Add(ref total, nums[i]);
            });
            millisecondiLock = watch.ElapsedMilliseconds;
            Console.WriteLine($"La somma dei primi {Numbers} è: {total} e con Interlocked impiego: {millisecondiLock}ms");
            speed = (double)millisecondiLock / millisecondiSequenziale;
            Console.WriteLine($"Speed: {speed}");
            //https://learn.microsoft.com/enus/dotnet/api/system.threading.tasks.parallel.for?view=net-7.0
            //https://learn.microsoft.com/enus/dotnet/api/system.threading.tasks.parallel.for?f1url=%3FappId%3DDev16IDEF1%26l%3DENUS%26k%3Dk(System.Threading.Tasks.Parallel.For%2560%25601)%3Bk(DevLang-csharp)%26rd%3Dtrue&view=net7.0#system-threading-tasks-parallel-for-1(system-int32-system-int32-system-threading-tasksparalleloptions-system-func((-0))-system-func((system-int32-system-threading-tasks-parallelloopstate0-0))-system-action((-0)))
            // Use type parameter to make subtotal a long, not an int
            //()=>0 inizializza la variabile locale al Task, subtotal in questo caso. La variabile subtotal viene inizializzata a 0
            total = 0;
            watch.Restart();
            Parallel.For<long>(0, nums.Length, () => 0, (j, loopState, subtotal) =>
            {
                subtotal += nums[j];//modify local variable
                return subtotal;
            },
            // value to be passed to next iteration
            //la variabile x è il risultato dell'elaborazione del Task. Corrisponde a subtotal quando il task finisce la sua elaborazione
            //Questa è la action che viene eseguita alla fine dell'elaborazione del Task
            (x) => Interlocked.Add(ref total, x));
            watch.Stop();
            var millisecondiOptimizate = watch.ElapsedMilliseconds;
            Console.WriteLine($"La somma dei primi {Numbers} è: {total} e con Interlocked.Add impiego: {millisecondiOptimizate}ms");
            speed = (double)millisecondiOptimizate / millisecondiSequenziale;
            Console.WriteLine($"Speed: {speed}");

            //confrontiamo il risultato con la formula di Gauss
            total = 0;
            watch.Restart();
            long sommaGauss = (long)Numbers * (Numbers + 1) / 2;
            watch.Stop();
            var millisecondiGauss = watch.ElapsedMilliseconds;
            Console.WriteLine($"La somma dei primi {Numbers} è: {sommaGauss} e con la formula di Gauss impiego: {millisecondiGauss}ms");
            speed = (double)millisecondiGauss / millisecondiSequenziale;
            Console.WriteLine($"Speed: {speed}");
        }
    }
}