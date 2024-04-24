using System.Text;
using System.Text.Json;
using HttpProxyControl;
namespace JSONDocumentWikiSummaryDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string wikiUrl = "https://it.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro&explaintext&exsectionformat=plain&redirects=1&titles=Dante_Alighieri";
            HttpClient client = HttpProxyHelper.CreateHttpClient(setProxy: true);
            //ottengo la risposta da Wikipedia come stringa
            string wikiSummaryJSON = await client.GetStringAsync(wikiUrl);
            string summary = ExtractSummaryFromJSON(wikiSummaryJSON);
            Console.OutputEncoding = Encoding.Unicode;
            Console.WriteLine(summary);
        }
        /// <summary>
        /// Estrae il campo summary dal JSON restituito dall'Action API di Wikipedia
        /// </summary>
        /// <param name="wikiSummary">il summary in formato JSON</param>
        /// <returns>La stringa corrispondente al contenuto del campo extract, stringa vuota se non riesce a fare l'estrazione</returns>
        private static string ExtractSummaryFromJSON(string wikiSummary)
        {
            using JsonDocument document = JsonDocument.Parse(wikiSummary);
            JsonElement root = document.RootElement;
            JsonElement query = root.GetProperty("query");
            JsonElement pages = query.GetProperty("pages");
            //per prendere il primo elemento dentro pages, creo un enumeratore delle properties
            JsonElement.ObjectEnumerator enumerator = pages.EnumerateObject();
            //quando si crea un enumeratore su una collection, si deve farlo avanzare di una posizione per portarlo sul primo elemento della collection
            //il primo elemento corrisponde all'id della pagina all'interno dell'oggetto pages
            if (enumerator.MoveNext())
            {
                //accedo all'elemento
                JsonElement targetJsonElem = enumerator.Current.Value;
                if (targetJsonElem.TryGetProperty("extract", out JsonElement extract))
                {
                    return extract.GetString() ?? string.Empty;
                }
            }
            return string.Empty;
        }
    }
}
