using WikitextExtensions;
using HttpProxyControl;
using System.Text.Json;
namespace Wiki
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = HttpProxyHelper.CreateHttpClient(setProxy: true);
            string url = "https://it.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro&explaintext&exsectionformat=plain&redirects=1&titles=Christopher_Nolan";
            string pagina = await client.GetStringAsync(url);
            string summary = Summary(pagina);
            Console.WriteLine(summary);
        }
        public static string Summary(string page)
        {
            JsonDocument vr = JsonDocument.Parse(page);
            JsonElement root = vr.RootElement;
            JsonElement query = root.GetProperty("query");
            JsonElement pages = query.GetProperty("pages");
            JsonElement.ObjectEnumerator en = pages.EnumerateObject();
            if(en.MoveNext())
            {
                JsonElement ele = en.Current.Value;
                if(ele.TryGetProperty("extract", out JsonElement contenuto))
                {
                    return contenuto.GetString() ?? string.Empty;
                }    
            }
            return string.Empty;
        }
    }
}
