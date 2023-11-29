namespace ParallelForDemo
{
    class Program
    {
        public static void Main()
        {
            long totalSize = 0;
            //args è fatto in questo modo: args[0] è il nome del programma,
            //args[1] è il primo parametro dopo il nome del programma,
            //args[2] è il secondo parametro dopo il nome del programma
            String[] args = Environment.GetCommandLineArgs();
            if (args.Length == 1)
            {
                Console.WriteLine("There are no command line arguments.");
                return;
            }
            if (!Directory.Exists(args[1]))
            {
                Console.WriteLine("The directory does not exist.");
                return;
            }
            //se sono a questo punto del codice significa che è stato passato un argomento
            String[] files = Directory.GetFiles(args[1]);
            //Parallel.For(valore iniziale incluso, valore finale escluso, lambda(indice) => {}) indice varia tra il valore iniziale e quello finale
            Parallel.For(0, files.Length,
                (index) => {
                FileInfo fi = new FileInfo(files[index]);
                long size = fi.Length;
                Interlocked.Add(ref totalSize, size);
                });
            //la lambda è assegata a task differenti
            Console.WriteLine("Directory '{0}':", args[1]);
            Console.WriteLine("{0:N0} files, {1:N0} bytes", files.Length, totalSize);
        }
    }
}