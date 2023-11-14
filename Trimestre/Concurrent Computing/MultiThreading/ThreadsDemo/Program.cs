namespace ThreadsDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //initialize a thread class object 
            //And pass your custom method name to the constructor parameter
            Thread t1 = new(SomeMethod1)
            {
                Name = "My Parallel Thread",

                Priority = ThreadPriority.BelowNormal //Influenza la politica dello schedulatore
            };

            //start running your thread
            t1.Start();
            //while thread is running in parallel 
            //you can carry out other operations here
            t1.Join();//Metodo bloccante -> aspetta che il thread t finisca
            Console.WriteLine("Press Enter to terminate!");
            Console.ReadLine();
            Thread t2 = new(SomeMethod2)
            {
                IsBackground = true//👈👈👈 Background thread -- see what happens with and without the property
            };
            //start running your thread
            t2.Start();
            Console.WriteLine("Main thread exits");
        }
        private static void SomeMethod1()
        {
            //your code here that you want to run parallel
            //most of the time it will be a CPU bound operation
            Console.WriteLine("Hello World!");
        }
        private static void SomeMethod2()
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Still working");
            Thread.Sleep(1000);//👈👈👈 just make this thread sleep for a certain amount of milliseconds
            Console.WriteLine("Just finished");//Non lo vedo perché il processo padre è finito
        }
    }
}