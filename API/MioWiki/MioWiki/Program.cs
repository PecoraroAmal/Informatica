using System.Text;
using System.Text.Json;
using HttpProxyControl;
namespace MioWiki
{

    class Program
    {
        static async Task Main(string[] args)
        {
            string wikiUrl = "https://it.wikipedia.org/w/api.php?action=parse&format=json&page=Daniel_Craig&prop=sections&disabletoc=1";
            HttpClient client = HttpProxyHelper.CreateHttpClient(setProxy: true);
            string wikiSummaryJSON = await client.GetStringAsync(wikiUrl);
            int summary = ExtractSummaryFromJSON(wikiSummaryJSON, "collegamenti");
            Console.OutputEncoding = Encoding.Unicode;
            Console.WriteLine(summary);
        }
        private static int ExtractSummaryFromJSON(string wikiSummary, string keyword)
        {
            using JsonDocument document = JsonDocument.Parse(wikiSummary);
            JsonElement root = document.RootElement;
            JsonElement parse = root.GetProperty("parse");
            JsonElement sections = parse.GetProperty("sections");
            for (int i = 0; i < sections.GetArrayLength(); i++)
            {
                JsonElement section = sections[i];
                if (section.TryGetProperty("line", out JsonElement lineElement) && lineElement.GetString()?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true)
                {
                    Console.WriteLine(i);
                    return i;
                }
            }
            return -1;

        }

    }
}
