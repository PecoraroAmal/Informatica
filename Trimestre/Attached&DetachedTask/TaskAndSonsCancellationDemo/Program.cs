using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
namespace TaskAndSonsCancellationDemo
{
    public class Program
    {
        public static async Task Main()
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            // Store references to the tasks so that we can wait on them and
            // observe their status after cancellation.
            Task t;
            //https://docs.microsoft.com/enus/dotnet/api/system.collections.concurrent.concurrentbag-1
            var tasks = new ConcurrentBag<Task>();
            Console.WriteLine("Press any key to begin tasks...");
            Console.ReadKey(true);
            Console.WriteLine("To terminate the example, press 'c' to cancel and exit...");


            Console.WriteLine();
            // Request cancellation of a single task when the token source is canceled.
            // Pass the token to the user delegate, and also to the task so itcan
            // handle the exception correctly.
            t = Task.Run(() => DoSomeWork(1, token), token);
            Console.WriteLine("Task {0} executing", t.Id);
            tasks.Add(t);
            // Request cancellation of a task and its children. Note the tokenis passed
            // to (1) the user delegate and (2) as the second argument to Task.Run, so
            // that the task instance can correctly handle the OperationCanceledException.
            t = Task.Run(() =>
            {
                // Create some cancelable child tasks.
                Task tc;
                for (int i = 3; i <= 10; i++)
                {
                    // For each child task, pass the same token
                    // to each user delegate and to Task.Run.
                    tc = Task.Run(() => DoSomeWork(i, token), token);
                    Console.WriteLine("Task {0} executing", tc.Id);
                    tasks.Add(tc);
                    // Pass the same token again to do work on the parent task.
                    // All will be signaled by the call to tokenSource.Cancel below.
                    //DoSomeWork(2, token);
                }
            }, token);
            Console.WriteLine("Task {0} executing", t.Id);
            tasks.Add(t);
            // Request cancellation from the UI thread.
            char ch = Console.ReadKey().KeyChar;
            if (ch == 'c' || ch == 'C')
            {
                tokenSource.Cancel();
                Console.WriteLine("\nTask cancellation requested.");
                // Optional: Observe the change in the Status property on the task.
                // It is not necessary to wait on tasks that have canceled. However,
                // if you do wait, you must enclose the call in a try-catch block to
                // catch the TaskCanceledExceptions that are thrown. If you do
                // not wait, no exception is thrown if the token that was passed to the
                // Task.Run method is the same token that requested the cancellation.
            }
            try
            {
                await Task.WhenAll(tasks.ToArray());
            }
            //in alternativa all’uso di await si può usare una Wait, ma in tal caso bisogna gestire l’AggregateException
            //try
            //{
            // Task myTask = Task.WhenAll(tasks.ToArray());
            // myTask.Wait();
            //}
            //catch (AggregateException ae)
            //{
            // foreach (var e in ae.Flatten().InnerExceptions)
            // {
            // if (e is TaskCanceledException)
            // {
            // Console.WriteLine($"\n{nameof(TaskCanceledException)}thrown\n");
            // }
            // else
            // {
            // throw;
            // }
            // }
            //}
            catch (OperationCanceledException)
            {
                Console.WriteLine($"\n{nameof(OperationCanceledException)}thrown\n");
            }
            finally
            {
                tokenSource.Dispose();
            }
            // Display status of all tasks.
            foreach (var task in tasks)
                Console.WriteLine("Task {0} status is now {1}", task.Id,
               task.Status);
        }
        static void DoSomeWork(int taskNum, CancellationToken ct)
        {
            // Was cancellation already requested?
            if (ct.IsCancellationRequested)
            {
                Console.WriteLine("Task {0} was cancelled before it got started.", taskNum);
                ct.ThrowIfCancellationRequested();
            }
            int maxIterations = 100;
            // NOTE!!! A "TaskCanceledException was unhandled
            // by user code" error will be raised here if "Just My Code"
            // is enabled on your computer. On Express editions JMC is
            // enabled and cannot be disabled. The exception is benign.
            // Just press F5 to continue executing your code.
            for (int i = 0; i <= maxIterations; i++)
            {
                // Do a bit of work. Not too much.
                var sw = new SpinWait();
                for (int j = 0; j <= 100; j++)
                    //https://stackoverflow.com/questions/1091135/whats-thepurpose-of-thread-spinwait-method
                    sw.SpinOnce();
                if (ct.IsCancellationRequested)
                {
                    Console.WriteLine("Task {0} cancelled", taskNum);
                    ct.ThrowIfCancellationRequested();
                }
            }
        }
    }
}