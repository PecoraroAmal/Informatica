using System.Collections.Concurrent;

namespace TaskParallelismControl
{
    namespace TaskParallelismControl
    {
        //Questa classe implementa metodi di estensione:
        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/how-to-implement-and-call-a-custom-extension-method
        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods

        public static class TaskConcurrencyHelper
        {
            //esempio preso da 
            //https://medium.com/@nirinchev/executing-a-collection-of-tasks-in-parallel-with-control-over-the-degree-of-parallelism-in-c-508d59ddffc6
            //con alcuni adattamenti
            /// <summary>
            /// Permette di eseguire in parallelo un metodo che restituisce un Task su una collection, specificando il numero massimo di task da impiegare
            /// </summary>
            /// <typeparam name="T">Tipo degli elementi della collection</typeparam>
            /// <param name="collection">Collection da processare</param>
            /// <param name="processor">metodo da eseguire su ciascun elemento della collection</param>
            /// <param name="degreeOfParallelism">Grado di parallelismo</param>
            /// <returns>Restituisce un task che termina quando tutti i task che processano la collection hanno terminato</returns>
            public static async Task ExecuteInParallel<T>(this IEnumerable<T> collection,
                                               Func<T, Task> processor,
                                               int degreeOfParallelism)
            {
                //creo una coda Thrade-safe a partire dalla collection
                var queue = new ConcurrentQueue<T>(collection);
                //creo tanti task (esecutori) quanto è il grado di parallelismo richiesto
                var tasks = Enumerable.Range(0, degreeOfParallelism).Select(async _ =>
                {
                    //ogni task cerca di svuotare la coda, prelevando un elemento e processandolo
                    while (queue.TryDequeue(out T? item))
                    {
                        await processor(item);
                    }
                });
                //attendo che tutti i task finiscano
                await Task.WhenAll(tasks);
            }

            public static async Task<ConcurrentBag<Tresult>> ExecuteInParallel<T, Tresult>(this IEnumerable<T> collection,
                                               Func<T, Task<Tresult>> processor,
                                               int degreeOfParallelism)
            {
                //creo una coda Thrade-safe a partire dalla collection
                var queue = new ConcurrentQueue<T>(collection);
                //creo una collection Thrade-safe dove inserire i risultati dell'elaborazione dei Task
                var results = new ConcurrentBag<Tresult>();

                var tasks = Enumerable.Range(0, degreeOfParallelism).Select(async _ =>
                {
                    //ogni task cerca di svuotare la coda, prelevando un elemento e processandolo
                    while (queue.TryDequeue(out T? item))
                    {
                        results.Add(await processor(item));
                    }
                });
                //attendo che tutti i task finiscano
                await Task.WhenAll(tasks);
                return results;

            }
        }
    }
}
