using System;
using WikitextExtensions;
using HttpProxyControl;
using System.Text.Json;
namespace MioWikiParse
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Console.Out.WriteLineAsync("Con il metodo");
            HttpClient client = HttpProxyHelper.CreateHttpClient(setProxy:true);
            string link = $"https://it.wikipedia.org/w/api.php?action=parse&format=json&page=Daniel_Craig&prop=wikitext&section=0&disabletoc=1";
            string wikiSummaryJSON = await client.GetStringAsync(link);
            string summary = ExtractBioFromJSON(wikiSummaryJSON);
            //await Console.Out.WriteLineAsync(summary);
            try
            {
                string[] readableText = summary.WikiTextToReadableTextNoSpace().SplitOnPeriod();
                foreach (var item in readableText)
                {
                    Console.WriteLine(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        private static string ExtractBioFromJSON(string wikiSummaryJSON)
        {
            using JsonDocument document = JsonDocument.Parse(wikiSummaryJSON);
            JsonElement root = document.RootElement;
            JsonElement parse = root.GetProperty("parse");
            JsonElement wikitext = parse.GetProperty("wikitext");

            if (wikitext.TryGetProperty("*", out JsonElement bioElement))
            {
                string bio = bioElement.GetString();
                if (!string.IsNullOrEmpty(bio))
                {
                    return bio;
                }
            }
            return string.Empty;
        }

    }
}
