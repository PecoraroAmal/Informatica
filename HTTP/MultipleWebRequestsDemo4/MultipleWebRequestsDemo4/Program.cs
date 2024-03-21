using System.Diagnostics;
using System.Runtime.InteropServices;

//using di librerie proprie
using TaskParallelismControl;
using HttpProxyControl;
using System.Collections.Concurrent;
using TaskParallelismControl.TaskParallelismControl;


namespace MultipleWebRequestsDemo4
{
    class Program
    {

        /// <summary>
        /// Scarica un file dalla rete e restituisce la lunghezza in byte
        /// </summary>
        /// <param name="url"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        static async Task<int> ProcessURLAsync(string url, HttpClient client)
        {
            var sw = new Stopwatch();
            sw.Start();
            var byteArray = await client.GetByteArrayAsync(url);
            sw.Stop();
            DisplayResults(url, "https://docs.microsoft.com/en-us/", byteArray, sw.ElapsedMilliseconds);
            return byteArray.Length;
        }
        /// <summary>
        /// Stampa una parte dell'url, la dimensione in byte di una pagina e il tempo impiegato per il download
        /// </summary>
        /// <param name="url"></param>
        /// <param name="urlHeadingStrip"></param>
        /// <param name="content"></param>
        /// <param name="elapsedMillis"></param>
        static void DisplayResults(string url, string urlHeadingStrip, byte[] content, long elapsedMillis)
        {
            // Display the length of each website. 
            var bytes = content.Length;
            // Strip off the "urlHeadingStrip" part from url
            var displayURL = url.Replace(urlHeadingStrip, "");

            Console.WriteLine($"\n{displayURL,-80} bytes: {bytes,-10} ms: {elapsedMillis,-10}");

        }
        /// <summary>
        /// Restituisce una lista di url
        /// </summary>
        /// <returns></returns>
        static List<string> SetUpURLList()
        {
            List<string> urls = new()
            {
                "https://docs.microsoft.com/en-us/welcome-to-docs",
                "https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/",
                "https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/",
                "https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/linq-to-objects",
                "https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/linq-and-strings",
                "https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/linq-to-xml-overview",
                "https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/async-return-types",
                "https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/linq-to-xml-vs-dom",
                "https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/reflection"
            };
            return urls;
        }
        /// <summary>
        /// Effettua il setup di una lista di url e per ognuno di essi avvia un download asincrono su un task separato
        /// </summary>
        /// <returns></returns>
        static async Task SumPageSizesAsync()
        {
            // Make a list of web addresses.
            List<string> urlList = SetUpURLList();
            //setup del client con eventuale Proxy
            HttpClient client = HttpProxyHelper.CreateHttpClient(setProxy: true);

            //misuriamo il tempo complessivo per scaricare tutte le pagine
            var swGlobal = new Stopwatch();
            swGlobal.Start();
            //processiamo in parallelo una lista di URL
            //IEnumerable<Task<int>> downloadTasks = urlList.Select(u => ProcessURLAsync(u, client));
            //ConcurrentBag<int> bag = new ConcurrentBag<int>();
            //await urlList.ExecuteInParallel(async u => { bag.Add(await ProcessURLAsync(u, client)); }, 10);
            //var theTotal = bag.ToArray().Sum();
            //await urlList.ExecuteInParallel(async u => { await Task.Delay(10); }, 10);

            //definiamo il grado di parallelismo
            const int numberOfParallelThreads = 5;
            //processiamo tutti gli oggetti della collection con il grado di parallelismo massimo predefinito
            ConcurrentBag<int> concurrentBagOfResults = await urlList.ExecuteInParallel(u => ProcessURLAsync(u, client), numberOfParallelThreads);
            //sommiamo tutti i valori restituiti dai thread
            var theTotal = concurrentBagOfResults.ToArray().Sum();
            Console.WriteLine($"Somma = {theTotal}");
            swGlobal.Stop();
            long elapsedTotalMs = swGlobal.ElapsedMilliseconds;

            // Display the total count for all of the web addresses.
            Console.WriteLine($"\r\n\r\nTotal bytes returned:  {theTotal}\r\n");
            Console.WriteLine($"Tempo complessivo di scaricamento = {elapsedTotalMs}");

        }

        static async Task Main(string[] args)
        {
            //imposto la dimensione della console - vale solo per Windows
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WindowWidth = 120;
            }
            await SumPageSizesAsync();
        }
    }
}
