using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
namespace TaskCancellationDemo
{
    class Program
    {
        //usare async Task al posto di void se si usa await task al posto di task.Wait()
        static async Task Main(string[] args)
        {
            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;
            var task = Task.Run(() =>
            {
                // Were we already canceled?
                ct.ThrowIfCancellationRequested();
                bool moreToDo = true;
                while (moreToDo)
                {
                    // Poll on this property if you have to do
                    // other cleanup before throwing.
                    if (ct.IsCancellationRequested)
                    {
                        // Clean up here, then...
                        ct.ThrowIfCancellationRequested();
                    }
                }
            }, ct); // Pass same token to Task.Run.
            Thread.Sleep(1000);
            tokenSource.Cancel();
            // Just continue on this thread, or await with try-catch:
            try
            {
                //https://stackoverflow.com/questions/13140523/await-vs-task-wait-deadlock
                //https://stackoverflow.com/questions/7340309/throw-exception-inside-a-taskawait-vs-wait
                //task.Wait();
                await task;
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: { e.Message} ");
            }
            catch (AggregateException e)
            {
                Console.WriteLine($"{nameof(AggregateException)} thrown with message: { e.Message} ");
            }
            finally
            {
                tokenSource.Dispose();
            }
        }
    }
}