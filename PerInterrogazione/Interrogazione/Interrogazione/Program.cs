using HttpProxyControl;
using System.Text.Json;
namespace Interrogazione
{
    internal class Program
    {
        public class JsonCont
        {
            public int userId { get; set; }
            public int id { get; set; }
            public string title { get; set; }
            public string body { get; set; }
        }
        static async Task Main(string[] args)
        {
            HttpClient client = HttpProxyHelper.CreateHttpClient(setProxy: true);
            string url = "https://jsonplaceholder.typicode.com/posts";
            string contenuto = await client.GetStringAsync(url);
            await Console.Out.WriteLineAsync(contenuto);
            Deserializzazione(contenuto);
        }
        public static void Deserializzazione(string input)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            List<JsonCont>? jsonCont = JsonSerializer.Deserialize<List<JsonCont>>(input);
            Console.WriteLine("Deserializzazione"); //si potrebbe fare in un altro modo
            foreach (var item in jsonCont)
            {
                Console.WriteLine("------------------------------------------------");
                Console.WriteLine($"userId: {item.userId}");
                Console.WriteLine($"Id: {item.id}");
                Console.WriteLine($"title: {item.title}");
                Console.WriteLine($"body: {item.body}");
            }
        }
    }
}
