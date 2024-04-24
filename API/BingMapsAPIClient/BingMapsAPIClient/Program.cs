using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using BingMapsAPIClient;
using HttpProxyControl;

namespace BingMapsAPIClient
{
    public class BingMapsStore
    {
        [JsonPropertyName("api_key")]
        public string APIKeyValue { get; set; } = string.Empty;

    }
    class Program
    {
        static readonly BingMapsStore bingMapsStore = GetDataFromStore();
        static readonly string bingMapsAPIKey = bingMapsStore.APIKeyValue;
        static readonly HttpClient client = HttpProxyHelper.CreateHttpClient(setProxy: true);

        static BingMapsStore GetDataFromStore()
        {
            //il file è nella cartella che contiene la soluzione corrente
            //il file contiene un oggetto JSON del tipo:
            //{
            //    "api_key":"api_key_value"
            //}
            string keyStorePath = "../../../../../../../MyBingMapsStore.json";
            string store = File.ReadAllText(keyStorePath);
            BingMapsStore? bingMapsStore = JsonSerializer.Deserialize<BingMapsStore>(store);
            return bingMapsStore ?? new BingMapsStore();
        }

        static async Task EsempioDiFindLocationByAddress()
        {
            Console.WriteLine("Motodo: EsempioDiFindLocationByAddress");
            //https://docs.microsoft.com/en-us/bingmaps/rest-services/locations/find-a-location-by-address
            //esempio: https://dev.virtualearth.net/REST/v1/Locations/FR/{postalCode}/{locality}/{addressLine}?includeNeighborhood={includeNeighborhood}&include={includeValue}&maxResults={maxResults}&key={bingMapsAPIKey}
            //https://docs.microsoft.com/en-us/bingmaps/rest-services/locations/find-a-location-by-address#api-parameters
            string postalCode = "23876";
            string locality = HttpUtility.UrlEncode("Monticello Brianza");
            string addressLine = HttpUtility.UrlEncode("Via dei Mille, 27");
            int includeNeighborhood = 1;
            string includeValue = "ciso2";
            int maxResults = 5;
            string addressUrl = $"https://dev.virtualearth.net/REST/v1/Locations/IT/{postalCode}/{locality}/{addressLine}?includeNeighborhood={includeNeighborhood}&include={includeValue}&maxResults={maxResults}&key={bingMapsAPIKey}";
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

        static async Task<Point?> FindLocationByAddress(string countryCode, string postalCode, string locality, string addressLine)
        {
            //https://docs.microsoft.com/en-us/bingmaps/rest-services/locations/find-a-location-by-address
            //esempio: https://dev.virtualearth.net/REST/v1/Locations/FR/{postalCode}/{locality}/{addressLine}?includeNeighborhood={includeNeighborhood}&include={includeValue}&maxResults={maxResults}&key={bingMapsAPIKey}
            //https://docs.microsoft.com/en-us/bingmaps/rest-services/locations/find-a-location-by-address#api-parameters

            locality = HttpUtility.UrlEncode(locality);
            addressLine = HttpUtility.UrlEncode(addressLine);
            int includeNeighborhood = 1;
            string includeValue = "ciso2";
            int maxResults = 1;
            string addressUrl = $"https://dev.virtualearth.net/REST/v1/Locations/{countryCode}/{postalCode}/{locality}/{addressLine}?includeNeighborhood={includeNeighborhood}&include={includeValue}&maxResults={maxResults}&key={bingMapsAPIKey}";
            Point? point = null;
            try
            {
                HttpResponseMessage response = await client.GetAsync(addressUrl);
                Location? data = await response.Content.ReadFromJsonAsync<Location>();
                point = data?.ResourceSets?[0].Resources?[0].Point;
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException || ex is ArgumentException)
                {
                    Debug.WriteLine(ex.Message + "\nIl recupero dei dati dal server non è riuscito");
                }
            }
            return point;
        }

        static async Task EsempioDiFindLocationByQuery()
        {
            Console.WriteLine("Motodo: EsempioDiFindLocationByQuery");
            //https://docs.microsoft.com/en-us/bingmaps/rest-services/locations/find-a-location-by-query
            //esempio: https://dev.virtualearth.net/REST/v1/Locations/{locationQuery}?includeNeighborhood={includeNeighborhood}&maxResults={maxResults}&include={includeValue}&key={BingMapsAPIKey}
            //https://docs.microsoft.com/en-us/bingmaps/rest-services/locations/find-a-location-by-query#api-parameters
            int includeNeighborhood = 1;
            string includeValue = "queryParse,ciso2";
            int maxResults = 5;
            string locationQuery = HttpUtility.UrlEncode("Via dei Mille 27, 23876 Monticello Brianza (Lc), Italia");
            string addressUrl = $"https://dev.virtualearth.net/REST/v1/Locations/{locationQuery}?includeNeighborhood={includeNeighborhood}&maxResults={maxResults}&include={includeValue}&key={bingMapsAPIKey}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(addressUrl);
                Location? data = await response.Content.ReadFromJsonAsync<Location>();
                Point? point = data?.ResourceSets?[0].Resources?[0].Point;
                if (point != null)
                {
                    //per stampare a console usiamo i web defaults
                    JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true };
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

        static async Task<Point?> FindLocationByQuery(string queryString)
        {
            //https://docs.microsoft.com/en-us/bingmaps/rest-services/locations/find-a-location-by-query
            //esempio: https://dev.virtualearth.net/REST/v1/Locations/{locationQuery}?includeNeighborhood={includeNeighborhood}&maxResults={maxResults}&include={includeValue}&key={BingMapsAPIKey}
            //https://docs.microsoft.com/en-us/bingmaps/rest-services/locations/find-a-location-by-query#api-parameters
            int includeNeighborhood = 1;
            string includeValue = "queryParse,ciso2";
            int maxResults = 1;
            string locationQuery = HttpUtility.UrlEncode(queryString);
            string addressUrl = $"https://dev.virtualearth.net/REST/v1/Locations/{locationQuery}?includeNeighborhood={includeNeighborhood}&maxResults={maxResults}&include={includeValue}&key={bingMapsAPIKey}";
            Point? point = null;
            try
            {
                HttpResponseMessage response = await client.GetAsync(addressUrl);
                Location? data = await response.Content.ReadFromJsonAsync<Location>();
                point = data?.ResourceSets?[0].Resources?[0].Point;
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException || ex is ArgumentException)
                {
                    Console.WriteLine(ex.Message + "\nIl recupero dei dati dal server non è riuscito");
                }

            }
            return point;
        }

        /// <summary>
        /// Scrive un file nel percorso di destinazione a partire da uno stream di input
        /// </summary>
        /// <param name="filePathDestination"></param>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        static async Task WriteBinaryAsync(string filePathDestination, Stream inputStream, int writeBufferSize = 4096)
        {
            using FileStream outputStream = new FileStream(filePathDestination,
                FileMode.Create, FileAccess.Write, FileShare.None,
                bufferSize: writeBufferSize, useAsync: true);
            await inputStream.CopyToAsync(outputStream);
        }
        static async Task ScaricaMappaDaBingMaps(string mapUrl, string pathToFile)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(mapUrl);
                response.EnsureSuccessStatusCode();
                await using Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
                //copio in modalità async il file 
                await WriteBinaryAsync(pathToFile, streamToReadFrom);
            }
            catch (HttpRequestException e)
            {

                Debug.WriteLine("\nException Caught!");
                Debug.WriteLine("Message :{0} ", e.Message);
            }
        }

        static async Task ScaricaMappaStatica(string center, string pathToFile)
        {
            //https://docs.microsoft.com/en-us/bingmaps/rest-services/imagery/get-a-static-map
            string mapCenter = HttpUtility.UrlEncode(center);
            int zoomLevel = 17;
            string mapSize = HttpUtility.UrlEncode("1024, 1024");
            string mapUrl = $"https://dev.virtualearth.net/REST/v1/Imagery/Map/Aerial/{mapCenter}/{zoomLevel}?mapSize={mapSize}&key={bingMapsAPIKey}";
            await ScaricaMappaDaBingMaps(mapUrl, pathToFile);
        }
        static async Task EsempioDiScaricamentoMappaStaticaByAddress()
        {
            //scarichiamo una mappa che ha come centro il punto definito mediante l'indirizzo dato
            Point? punto = await FindLocationByAddress("IT", "23876", "Monticello Brianza (Lc)", "Via dei Mille, 27");
            if (punto != null)
            {
                Console.WriteLine($"Le coordinate del punto corrispondente all'indirizzo fornito sono (lat, lon): {punto.Coordinates[0]}, {punto.Coordinates[1]}");
                string fileName = "mappaByAddress.jpg";
                Console.WriteLine($"Scarico una mappa statica centrata sul punto. Controlla sul desktop l'immagine {fileName}");
                string pathToFile = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + Path.DirectorySeparatorChar + fileName;
                FormattableString centerCoordinates = $"{punto.Coordinates[0]},{punto.Coordinates[1]}";
                await ScaricaMappaStatica(FormattableString.Invariant(centerCoordinates), pathToFile);
            }

        }


        static async Task EsempioDiScaricamentoMappaStaticaByQuery()
        {
            string query = "Via dei Mille 27, 23876 Monticello Brianza (Lc), Italia";
            Point? punto = await FindLocationByQuery(query);
            if (punto != null)
            {
                Console.WriteLine($"Le coordinate del punto corrispondente all'indirizzo fornito sono (lat, lon): {punto.Coordinates[0]}, {punto.Coordinates[1]}");
                string fileName = "mappaByQuery.jpg";
                Console.WriteLine($"Scarico una mappa statica centrata sul punto. Controlla sul desktop l'immagine {fileName}");
                string pathToFile = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + Path.DirectorySeparatorChar + fileName;
                FormattableString centerCoordinates = $"{punto.Coordinates[0]},{punto.Coordinates[1]}";
                await ScaricaMappaStatica(FormattableString.Invariant(centerCoordinates), pathToFile);
            }

        }
        static async Task Main(string[] args)
        {
            await EsempioDiFindLocationByAddress();

            await EsempioDiFindLocationByQuery();

            await EsempioDiScaricamentoMappaStaticaByAddress();

            await EsempioDiScaricamentoMappaStaticaByQuery();

        }
    }
}