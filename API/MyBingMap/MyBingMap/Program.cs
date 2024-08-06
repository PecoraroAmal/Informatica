using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using BingMapsAPIClient;
using HttpProxyControl;
namespace MyBingMap
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = HttpProxyHelper.CreateHttpClient(setProxy:true);
            string key = "AksnkPgX_iXcVsrE7ej3jjSxlwWg1OTa0KuBCYcZQD2ciMfSPjVdQtOT7MMgM_-g";
            Console.WriteLine("Stampare le invo a partire dal paese e via");
            await Test1(key, client);
        }
        static async Task Test1(string key, HttpClient client)
        {
            string postalCode = "23876";
            string locality = HttpUtility.UrlEncode("Monticello Brianza");
            string addressLine = HttpUtility.UrlEncode("Via dei Mille, 27");
            int includeNeighborhood = 1;
            string includeValue = "ciso2";
            int maxResults = 5;
            string addressUrl = $"https://dev.virtualearth.net/REST/v1/Locations/IT/{postalCode}/{locality}/{addressLine}?includeNeighborhood={includeNeighborhood}&include={includeValue}&maxResults={maxResults}&key={key}";
            try
            {
                HttpResponseMessage response = await client.GetAsync(addressUrl);
                Location? data = await response.Content.ReadFromJsonAsync<Location>();
                Point? point = data?.ResourceSets?[0].Resources?[0].Point;
                if (data != null && point != null)
                {
                    //per stampare a console usiamo i web defaults
                    JsonSerializerOptions options = new(JsonSerializerDefaults.Web) { WriteIndented = true };
                    Console.WriteLine($"Dati recuperati dall'endpoint:\n {JsonSerializer.Serialize(data, options)}");
                    Console.WriteLine($"Coordinate del punto: lat = {point.Coordinates[0]}, lon = {point.Coordinates[1]}");
                }
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException || ex is ArgumentException)
                {
                    Debug.WriteLine(ex.Message + "\nIl recupero dei dati dal server non è riuscito");
                }
            }
        }


    }
}