namespace TaskIntro
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "Main";
            // Create a task and supply a user delegate by using a lambda expression.
            Task taskA = new Task(() => Console.WriteLine("Hello from taskA."));
            // Start the task.
            taskA.Start();
            // Output a message from the calling thread.
            Console.WriteLine("Hello from thread '{0}'.",Thread.CurrentThread.Name);
            taskA.Wait(); //= alla join, attendiamo la fine della task
        }
    }
}