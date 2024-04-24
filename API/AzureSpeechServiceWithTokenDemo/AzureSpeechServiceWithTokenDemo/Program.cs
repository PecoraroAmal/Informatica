using System.Text.Json.Serialization;
using System.Text.Json;
using HttpProxyControl;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Text;
namespace AzureSpeechServiceWithTokenDemo
{
    public class AzureSpeechServiceStore
    {
        [JsonPropertyName("api_key")]
        public string APIKeyValue { get; set; } = string.Empty;

        [JsonPropertyName("location_region")]
        public string LocationRegion { get; set; } = string.Empty;

        [JsonPropertyName("endpoint")]
        public string EndPoint { get; set; } = string.Empty;
    }

    internal class Program
    {
        static readonly string APIKeyName = "Ocp-Apim-Subscription-Key";
        static readonly AzureSpeechServiceStore azureSpeechServiceStore = GetDataFromStore();
        static readonly string azureSpeechServiceKey = azureSpeechServiceStore.APIKeyValue;
        static readonly string azureSpeechServiceRegion = azureSpeechServiceStore.LocationRegion;
        static readonly string azureSpeechEndPoint = azureSpeechServiceStore.EndPoint;

        static readonly HttpClient _client = HttpProxyHelper.CreateHttpClient(setProxy: true);

        static readonly object _lock = new();
        static string _token = string.Empty;
        static readonly bool languageAutoDetect = false;
        //questa property non sarebbe strettamente necessaria. Serve solo se si lavoro in ambito multi-treading
        static string Token
        {
            get
            {
                lock (_lock)
                {
                    return _token;
                }
            }
            set
            {
                lock (_lock)
                {
                    _token = value;
                }
            }
        }
        static AzureSpeechServiceStore GetDataFromStore()
        {
            //il file è nella cartella che contiene la soluzione corrente
            //il file contiene un oggetto JSON del tipo:
            //{
            //   "api_key": "api_key_value",
            //   "location_region": "location_region_value",
            //   "endpoint": "endpoint_value" 
            //}
            string keyStorePath = "../../../../../../../MyAzureVoiceActor.json";
            string store = File.ReadAllText(keyStorePath);
            AzureSpeechServiceStore? azureSpeechServiceStore = JsonSerializer.Deserialize<AzureSpeechServiceStore>(store);
            return azureSpeechServiceStore ?? new AzureSpeechServiceStore();
        }
        /// <summary>
        /// //effettua le POST per ottenere il Token
            // https://learn.microsoft.com/en-us/azure/ai-services/authentication#authenticate-with-an-access-token
        /// </summary>
        /// <returns>il token</returns>
        static async Task<string> GetToken()
        {
            //documentazione di riferimento
            // https://learn.microsoft.com/en-us/azure/ai-services/authentication#authenticate-with-an-access-token
            /*
             curl --ca-native -v -X POST `
            "https://YOUR-REGION.api.cognitive.microsoft.com/sts/v1.0/issueToken" `
            -H "Content-type: application/x-www-form-urlencoded" `
            -H "Content-length: 0" `
            -H "Ocp-Apim-Subscription-Key: api_key_value"
             */
            //string url = "https://YOUR-REGION.api.cognitive.microsoft.com/sts/v1.0/issueToken";

            _client.DefaultRequestHeaders.Add(APIKeyName, azureSpeechServiceKey);
            //nell'endpoint caricato da file c'è lo / finale
            HttpRequestMessage request = new(HttpMethod.Post, azureSpeechEndPoint + "sts/v1.0/issueToken")
            {
                Content = new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded")
            };
            try
            {
                // Esegue la richiesta POST
                HttpResponseMessage response = await _client.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                // Gestisce eventuali eccezioni durante la richiesta HTTP
                Console.WriteLine($"Error: {e.Message}");
            }
            return await Task.FromResult(string.Empty);
        }
        /// <summary>
        /// Aggiorna il token per il servizio speech
        /// </summary>
        /// <param name="state"></param>
        static async void UpdateToken(object state)
        {
            if (state is not SpeechConfig speechConfig)
            {
                return;
            }
            // Esegue il metodo GetToken per ottenere un nuovo token
            string newToken = await GetToken();
            // Aggiorna la proprietà Token con il nuovo token ottenuto
            Token = newToken;
            speechConfig.AuthorizationToken = Token;
            Console.WriteLine($"Token updated at {DateTime.Now}");
            Console.WriteLine($"New Token:  {Token}");
        }
        static void OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
        {
            switch (speechRecognitionResult.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    Console.WriteLine($"RECOGNIZED: Text={speechRecognitionResult.Text}");
                    break;
                case ResultReason.NoMatch:
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                    break;
                case ResultReason.Canceled:
                    var cancellation = CancellationDetails.FromResult(speechRecognitionResult);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                    }
                    break;
            }
        }
        async static Task Main(string[] args)
        {
            //acquisiamo il primo token
            Token = await GetToken();
            //configuriamo il servizio Speech
            //var speechConfig = SpeechConfig.FromSubscription(azureSpeechServiceKey, azureSpeechServiceRegion);
            var speechConfig = SpeechConfig.FromAuthorizationToken(authorizationToken: Token, azureSpeechServiceRegion);
            //facciamo partire un timer che periodicamente aggiorna speechConfig.AuthorizationToken
            //per aggiornare il token occorre impostare la property speechConfig.AuthorizationToken = currentToken;
            //in questa demo il timer parte dopo 5 secondi e poi ha un periodo di aggiornamento di 10 secondi
            //nella realtà il token restituito da Azure dura 10 minuti, quindi andrebbe impostato un timer che 
            //parte dopo, ad esempio 9 minuti e 90 secondi dal primo token e poi ha un periodo di aggiornamento
            //di circa 9 minuti e mezzo per avere un po' di margine rispetto alla scadenza
            Timer _timer = new(UpdateToken!, speechConfig, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10));

            using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            //impostiamo automaticamente il proxy se presente
            ProxyParams? proxyParams = HttpProxyHelper.GetHttpClientProxyParams();
            if (proxyParams.HasValue)
            {
                speechConfig.SetProxy(proxyParams.Value.ProxyAddress, proxyParams.Value.ProxyPort);
            }
            //se si vuole il riconoscimento automatico della lingua allo startup del servizio - impiega più tempo per riconoscere il testo nelle fasi iniziali
            if (languageAutoDetect)
            {
                //Language Autodetect at startup
                //https://learn.microsoft.com/en-us/azure/ai-services/speech-service/language-identification?pivots=programming-language-csharp&tabs=once#at-start-and-continuous-language-identification
                //up to 4 different cultures can be specified
                var autoDetectSourceLanguageConfig = AutoDetectSourceLanguageConfig.FromLanguages(["it-IT", "en-US", "fr-FR", "de-DE"]);
                using var speechRecognizer = new SpeechRecognizer(speechConfig, autoDetectSourceLanguageConfig, audioConfig);
                Console.WriteLine("Parla nel microfono.");
                var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
                OutputSpeechRecognitionResult(speechRecognitionResult);
            }
            else //configurazione con cultura pre-impostata
            {
                speechConfig.SpeechRecognitionLanguage = "it-IT";
                using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);
                Console.WriteLine("Parla nel microfono.");
                var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
                OutputSpeechRecognitionResult(speechRecognitionResult);
            }
            //questa parte serve solo a mostrare l'aggiornamento del timer
            Console.WriteLine("Aspettiamo che il timer scada.... non premere invio per vedere l'aggiornamento del token");
            Console.ReadLine();
        }
    }
}
