using HttpProxyControl;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using APIClientForPostmanMockServer.Model;

namespace APIClientForPostmanMockServer
{
    public class PostmanStore
    {
        [JsonPropertyName("api_key")]
        public string APIKeyValue { get; set; } = string.Empty;

        [JsonPropertyName("base_address")]
        public string BaseAddress { get; set; } = string.Empty;
    }
    class Program
    {
        const string APIKeyName = "X-API-Key";
        static readonly PostmanStore postmanStore = GetDataFromStore();
        static readonly string APIKeyValue = postmanStore.APIKeyValue;
        static readonly string baseAddress = postmanStore.BaseAddress;
        static readonly string acceptedMediaType = "application/json";
        static readonly HttpClient _client = HttpProxyHelper.CreateHttpClient(setProxy: true);
        /// <summary>
        /// Recupera i secrets da file
        /// </summary>
        /// <returns></returns>
        static PostmanStore GetDataFromStore()
        {
            //il file è nella cartella che contiene la soluzione corrente
            //il file contiene un oggetto JSON del tipo:
            //{
            //    "api_key":"api_key_value",
            //    "base_address": "base_address_value"
            //}
            string keyStorePath = "../../../../../../../MyPostmanStore.json";
            string store = File.ReadAllText(keyStorePath);
            PostmanStore? postmanStore = JsonSerializer.Deserialize<PostmanStore>(store);
            return postmanStore ?? new PostmanStore();
        }
        /// <summary>
        /// Configura i parametri dell'header per effettuare le richieste all'endpoint remoto
        /// Il base address del client viene configurato con il base URL del server
        /// https://swagger.io/docs/specification/2-0/api-host-and-base-path/
        /// L'header viene resettato e vengono aggiunti il media type e l'X-API-Key 
        /// </summary>
        static void ConfigureHttpClientHeaders()
        {
            _client.BaseAddress = new Uri(baseAddress);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptedMediaType));
            //https://stackoverflow.com/questions/53551361/how-to-add-api-key-in-request-header-using-web-api
            //https://stackoverflow.com/questions/14627399/setting-authorization-header-of-httpclient
            //l'istruzione seguente scrive nello header del messaggio http di richiesta la x-api-key
            _client.DefaultRequestHeaders.Add(APIKeyName, APIKeyValue);
        }
        static async Task Main(string[] args)
        {
            // https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.getawaiter 
            //GetAwaiter is intended for compiler use rather than use directly in code.
            //RunAsync().GetAwaiter().GetResult();
            await RunAsync();
        }
        static async Task RunAsync()
        {
            ConfigureHttpClientHeaders();

            try
            {
                Console.WriteLine("\n\n****************************************************************");
                Console.WriteLine("Test di POST");
                Console.WriteLine("Simuliamo la POST per inserire un prodotto sull'endpoint remoto");
                // Create a new product
                Product? product = new()
                {
                    Name = "Insalata verde",
                    Price = 1.55,
                    CompanyId = 34
                };
                //la POST dovrebbe restituire l'uri dell'oggetto creato e il suo id
                Uri? url = await PostProductAsync(product, _client);
                if (url != null)
                {
                    Console.WriteLine($"Created at {url}");
                    // Get the product
                    //con l'uri della risorsa creata sul server remoto recuperiamo l'oggetto
                    Console.WriteLine("Il prodotto caricato sul server mediante la POST non è realmente salvato, \n" +
                        "quindi quando recuperiamo il valore tramite la sua url otteniamo un valore diverso");
                    product = await GetProductAsync(url.PathAndQuery, _client);
                    if (product != null)
                    {
                        ShowProduct(product);
                        // Update the product
                        Console.WriteLine("\n\n****************************************************************");
                        Console.WriteLine("Test di PUT");
                        Console.WriteLine("Updating price...");
                        product.Price = 80;
                        Product? updatedProduct = await PutProductAsync(product, _client);
                        // Get the updated product
                        //Console.WriteLine("Il prodotto aggiornato");
                        //product = await GetProductAsync(url.PathAndQuery);
                        if (updatedProduct != null)
                        {
                            ShowProduct(updatedProduct);
                        }
                        // Delete the product
                        Console.WriteLine("\n\n****************************************************************");
                        Console.WriteLine("Test di DELETE");
                        Console.WriteLine($"Eliminiamo il prodotto con id = {product.Id}");
                        var statusCode = await DeleteProductAsync(product.Id, _client);
                        Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");
                    }

                }

                //otteniamo tutti i prodotti
                Console.WriteLine("\n\n****************************************************************");
                Console.WriteLine("Test di GET di tutti i prodotti");
                Console.WriteLine("Elenco dei prodotti");
                var products = await GetAllProductsAsync("products", _client);
                if (products != null)
                {
                    ShowProducts(products);
                }

                //dato un prodotto, ottenere i dati dell'azienda che produce tale prodotto
                Console.WriteLine("\n\n****************************************************************");
                Console.WriteLine("Dato un prodotto, ottenere i dati dell'azienda che produce tale prodotto");

                //Console.Write("Inserisci l'id di un prodotto: ");

                //int productId = int.Parse(Console.ReadLine() ?? "0");
                int productId = 4;
                Console.WriteLine($"Recuperiamo il prodotto con id = {productId}: ");
                string productUri = $"products/{productId}";
                product = await GetProductAsync(productUri, _client);
                if (product != null)
                {
                    JsonSerializerOptions options = new() { WriteIndented = true };
                    Console.WriteLine("il prodotto corrispondente all'id inserito è:");
                    Console.WriteLine(JsonSerializer.Serialize(product, options));
                    //si noti che il prodotto ottenuto non corrisponde a quello riportato con
                    //lo stesso id nell'elenco dei prodotti, poiché viene generato a caso ogni volta
                    string companyUri = $"companies/{product.CompanyId}";
                    Company? company = await GetCompanyAsync(companyUri, _client);
                    if (company != null)
                    {
                        Console.WriteLine("L'azienda che produce il prodotto scelto è:");
                        Console.WriteLine(JsonSerializer.Serialize(company, options));
                    }
                }

                Console.WriteLine("\n\n****************************************************************");
                Console.WriteLine("Elenco di tutte le compagnie");
                var companies = await GetAllCompaniesAsync("companies", _client);
                if (companies != null)
                {
                    JsonSerializerOptions options = new() { WriteIndented = true };
                    companies.ForEach(c => Console.WriteLine(JsonSerializer.Serialize(c, options) + "\n"));
                }
                Console.WriteLine("Test completato");

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("C'è stato un errore con la richiesta effettuata");
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }


        static void ShowProduct(Product product)
        {
            Console.WriteLine($"Id: {product.Id} Name: {product.Name,-50} Price: " +
                $"{product.Price}\tCompanyId: {product.CompanyId}");
        }
        static void ShowProducts(List<Product> products)
        {
            products.ForEach(p => ShowProduct(p));
        }
        #region REST_Methods_Products
        /// <summary>
        /// Effettua la POST di un nuovo prodotto sul server
        /// </summary>
        /// <param name="product">il prodotto da creare sul server</param>
        /// <returns>l'URI della risorsa creata, oppure null se l'uri non è fornito dal server</returns>
        /// <exception cref="HttpRequestException">Se lo status code restituito dall'endpoint corrisponde ad un errore</exception>
        static async Task<Uri?> PostProductAsync(Product product, HttpClient client)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("products", product);
            //verifico che l'operazione abbia avuto successo
            response.EnsureSuccessStatusCode();

            //sull'oggetto response posso fare diverse elaborazioni
            //se il server restituisce l'oggetto creato:
            //1)posso recuperarlo dal body della risposta
            Product? prodottoCreato = await response.Content.ReadFromJsonAsync<Product>();
            if (prodottoCreato != null)
            {
                Console.WriteLine("Prodotto creato:\n" + JsonSerializer.Serialize(prodottoCreato));
            }
            //2)oppure posso leggere il body della risposta come stringa e poi elaborarla
            string responseContentAsstring = await response.Content.ReadAsStringAsync();
            Product? prodotto = JsonSerializer.Deserialize<Product>(responseContentAsstring);
            if (prodotto != null)
            {
                Console.WriteLine("Prodotto creato come stringa\n" + responseContentAsstring);
            }
            //se il server restituisce l'uri della risorsa
            // return URI of the created resource.
            Uri? resourceUri = response.Headers.Location;
            if (resourceUri != null)
            {
                Console.WriteLine("Headers location: " + response.Headers.Location);
                return response.Headers.Location;
            }
            else
            {
                return null;
            }

        }

        //effettua la POST utilizzando il metodo PostAsync e l'oggetto StringContent
        static async Task<Uri?> PostProductAsync2(Product product, HttpClient client)
        {
            //trasformo l'oggetto in formato JSON
            string data = JsonSerializer.Serialize(product);
            //trasformo il JSON in uno StringContent
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            //faccio la Post asincrona dello StringContent
            var response = await client.PostAsync("products", content);
            //verifico che l'operazione sia andata a buon fine
            response.EnsureSuccessStatusCode();

            //sull'oggetto response posso fare diverse elaborazioni
            //se il server restituisce l'oggetto creato, posso recuperarlo dal body della risposta
            Product? prodottoCreato = await response.Content.ReadFromJsonAsync<Product>();
            Console.WriteLine("Prodotto creato:\n" + JsonSerializer.Serialize(prodottoCreato));
            //oppure posso leggere il body della risposta come stringa e poi elaborarla
            string prodottoCreatoAsString = await response.Content.ReadAsStringAsync();
            Product? prodotto = JsonSerializer.Deserialize<Product>(prodottoCreatoAsString);
            Console.WriteLine("Prodotto creato come stringa\n" + prodottoCreatoAsString);
            // return URI of the created resource.
            Console.WriteLine("Headers location: " + response.Headers.Location);
            return response.Headers.Location;
        }
        /// <summary>
        /// Effettua una GET sul server per recuperare un oggetto di cui è specificato l'id nella rotta
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Il prodotto </returns>
        static async Task<Product?> GetProductAsync(string path, HttpClient client)
        {
            Product? product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadFromJsonAsync<Product?>();
            }
            return product;
        }

        /// <summary>
        /// Effettua una GET sul server per richiedere tutti i prodotti. 
        /// In questo caso si utilizza il metodo GetStreamAsync di HttpClient in abbinamento al DeserializeAsync
        /// di JsonSerializer
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static async Task<List<Product>?> GetAllProductsAsync(string path, HttpClient client)
        {
            await using Stream stream = await client.GetStreamAsync(path);
            var products = await JsonSerializer.DeserializeAsync<List<Product>>(stream);
            return products;
        }
        static async Task<Product?> PutProductAsync(Product product, HttpClient client)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync($"products/{product.Id}", product);
            response.EnsureSuccessStatusCode();
            //Console.WriteLine("Aggiornamento effettuato");
            // Deserialize the updated product from the response body.
            Product? updatedProduct = await response.Content.ReadFromJsonAsync<Product?>();
            //Console.WriteLine("Prodotto aggiornato sul server:\n" + JsonSerializer.Serialize(updatedProduct));
            return updatedProduct;
        }
        static async Task<HttpStatusCode> DeleteProductAsync(int id, HttpClient client)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"products/{id}");
            return response.StatusCode;
        }
        #endregion REST_Methods_Products

        #region REST_Methods_Companies
        static async Task<Company?> GetCompanyAsync(string path, HttpClient client)
        {
            return await client.GetFromJsonAsync<Company>(path);
        }
        static async Task<List<Company>?> GetAllCompaniesAsync(string path, HttpClient client)
        {
            await using Stream stream = await client.GetStreamAsync(path);
            var companies = await JsonSerializer.DeserializeAsync<List<Company>>(stream);
            return companies;
        }
        static async Task<List<Company>?> GetAllCompaniesAsync2(string path, HttpClient client)
        {
            HttpResponseMessage response = await client.GetAsync(path);
            string? companies = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Company>?>(companies);
        }
        static async Task<List<Company>?> GetAllCompaniesAsync3(string path, HttpClient client) => await client.GetFromJsonAsync<List<Company>>(path);
        #endregion REST_Methods_Companies
    }
}