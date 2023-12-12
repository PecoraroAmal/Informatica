
namespace _4Ragazzi
{
    internal class Program
    {
        static bool fischio = true;
        static bool palla = false;
        static readonly object _lock = new object();
        static void Main(string[] args)
        {
            Task giovanni = Task.Run(() => Giovanni("Giovanni","Mattia"));
            Random rand = new Random();
            int gen = rand.Next(1000, 3000);
            Thread.Sleep(gen);
            fischio = false;
            Console.WriteLine($"L'allenatore ha fischiato, i ragazzi si sono passati la palla per {gen}ms");
        }

        private static void Giovanni(string possessore, string ricevente)
        {
            if (fischio)
            {
                lock (_lock)
                {
                    if (palla == false)
                    {
                        palla = true;
                        Random rand = new Random();
                        int gen = rand.Next(100, 301);
                        Console.WriteLine($"Sono {possessore} e passo la palla a {ricevente}");
                        Task.Delay(gen).Wait();
                        Task mattia = Task.Run(() => Mattia("Mattia", "Alessandro"));
                        palla = false;
                    }
                }
            }
        }
        private static void Mattia(string possessore, string ricevente)
        {
            if (fischio)
            {
                lock (_lock)
                {
                    if (palla == false)
                    {
                        palla = true;
                        Random rand = new Random();
                        int gen = rand.Next(100, 301);
                        Console.WriteLine($"Sono {possessore} e passo la palla a {ricevente}");
                        Task.Delay(gen).Wait();
                        Task mattia = Task.Run(() => Alessandro("Alessandro", "Roberto"));
                        palla = false;
                    }
                }
            }
        }
        private static void Alessandro(string possessore, string ricevente)
        {
            if (fischio)
            {
                lock (_lock)
                {
                    if (palla == false)
                    {
                        palla = true;
                        Random rand = new Random();
                        int gen = rand.Next(100, 301);
                        Console.WriteLine($"Sono {possessore} e passo la palla a {ricevente}");
                        Task.Delay(gen).Wait();
                        Task mattia = Task.Run(() => Roberto("Roberto", "Giovanni"));
                        palla = false;
                    }
                }
            }
        }
        private static void Roberto(string possessore, string ricevente)
        {
            if (fischio)
            {
                lock (_lock)
                {
                    if (palla == false)
                    {
                        palla = true;
                        Random rand = new Random();
                        int gen = rand.Next(100, 301);
                        Console.WriteLine($"Sono {possessore} e passo la palla a {ricevente}");
                        Task.Delay(gen).Wait();
                        Task mattia = Task.Run(() => Giovanni("Giovanni", "Mattia"));
                        palla = false;
                    }
                }
            }
        }
    }
}
