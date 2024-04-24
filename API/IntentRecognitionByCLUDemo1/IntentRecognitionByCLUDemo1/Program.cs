using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Intent;
using Microsoft.CognitiveServices.Speech;
using System.Text.Json.Serialization;
using System.Text.Json;
using IntentRecognitionByCLUDemo1.Models;
using HttpProxyControl;

namespace IntentRecognitionByCLUDemo1
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
    public class AzureIntentRecognitionByCLUStore
    {
        [JsonPropertyName("api_key")]
        public string KeyValue { get; set; } = string.Empty;

        [JsonPropertyName("location_region")]
        public string LanguageLocationRegion { get; set; } = string.Empty;

        [JsonPropertyName("language_endpoint")]
        public string LanguageEndpoint { get; set; } = string.Empty;

    }

    class Program
    {
        static readonly AzureSpeechServiceStore azureSpeechServiceStore = GetSpeechDataFromStore();
        static readonly string azureSpeechServiceKey = azureSpeechServiceStore.APIKeyValue;
        static readonly string azureSpeechServiceRegion = azureSpeechServiceStore.LocationRegion;

        static readonly AzureIntentRecognitionByCLUStore azureIntentRecognitionByCLUStore = GetCLUDataFromStore();
        static readonly string azureCluLanguageServiceKey = azureIntentRecognitionByCLUStore.KeyValue;
        static readonly string azureCluLanguageServiceEndpoint = azureIntentRecognitionByCLUStore.LanguageEndpoint;

        // Your CLU project name and deployment name.
        static string cluProjectName = "MyHome";
        static string cluDeploymentName = "HomeTest1";
        //static string cluProjectName = "MyResponder";
        //static string cluDeploymentName = "MyResponderDeployment1";
        static AzureIntentRecognitionByCLUStore GetCLUDataFromStore()
        {
            //{
            //    "api_key": "valore_chiave",
            //    "location_region":"valore_region",
            //    "language_endpoint": "valore_endpoint"
            //}
            string keyStorePath = "../../../../../../../MyAzureVoiceActor.json";
            string store = File.ReadAllText(keyStorePath);
            AzureIntentRecognitionByCLUStore? azureSpeechServiceStore = JsonSerializer.Deserialize<AzureIntentRecognitionByCLUStore>(store);
            return azureSpeechServiceStore ?? new AzureIntentRecognitionByCLUStore();
        }
        static AzureSpeechServiceStore GetSpeechDataFromStore()
        {
            //il file è nella cartella che contiene la soluzione corrente
            //il file contiene un oggetto JSON del tipo:
            // {
            //   "api_key": "api_key_value",
            //   "location_region: "location_region_value",
            //   "endpoint": "endpoint_value"       
            //}
            string keyStorePath = "../../../../../../../Test1.json";
            string store = File.ReadAllText(keyStorePath);
            AzureSpeechServiceStore? azureSpeechServiceStore = JsonSerializer.Deserialize<AzureSpeechServiceStore>(store);
            return azureSpeechServiceStore ?? new AzureSpeechServiceStore();
        }
        async static Task Main(string[] args)
        {
            var speechConfig = SpeechConfig.FromSubscription(azureSpeechServiceKey, azureSpeechServiceRegion);
            speechConfig.SpeechRecognitionLanguage = "en-US";
            //speechConfig.SpeechRecognitionLanguage = "it-IT";
            ProxyParams? proxyParams = HttpProxyHelper.GetHttpClientProxyParams();
            if (proxyParams.HasValue)
            {
                speechConfig.SetProxy(proxyParams.Value.ProxyAddress, proxyParams.Value.ProxyPort);
            }
            using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();

            // Creates an intent recognizer in the specified language using microphone as audio input.
            using var intentRecognizer = new IntentRecognizer(speechConfig, audioConfig);
            var cluModel = new ConversationalLanguageUnderstandingModel(
                azureCluLanguageServiceKey,
                azureCluLanguageServiceEndpoint,
                cluProjectName,
                cluDeploymentName);
            var collection = new LanguageUnderstandingModelCollection
                {
                    cluModel
                };
            intentRecognizer.ApplyLanguageModels(collection);

            Console.WriteLine("Speak into your microphone.");
            var recognitionResult = await intentRecognizer.RecognizeOnceAsync().ConfigureAwait(false);

            // Checks result.
            if (recognitionResult.Reason == ResultReason.RecognizedIntent)
            {
                Console.WriteLine($"RECOGNIZED: Text={recognitionResult.Text}");
                Console.WriteLine($"    Intent Id: {recognitionResult.IntentId}.");
                Console.WriteLine($"    Language Understanding JSON: {recognitionResult.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult)}.");
                string jsonResponse = recognitionResult.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult);
                JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true };
                CLUResponse cluResponse = JsonSerializer.Deserialize<CLUResponse>(jsonResponse, options) ?? new CLUResponse();
                Console.WriteLine("Risultato deserializzato:");
                Console.WriteLine($"kind: {cluResponse.Kind}");
                Console.WriteLine($"result.query: {cluResponse.Result?.Query}");
                Console.WriteLine($"result.prediction.topIntent: {cluResponse.Result?.Prediction?.TopIntent}");
                Console.WriteLine($"result.prediction.entities: ");
                cluResponse.Result?.Prediction?.Entities?.ForEach(s => Console.WriteLine($"\tcategory = {s.Category}; text= {s.Text};"));
            }
            else if (recognitionResult.Reason == ResultReason.RecognizedSpeech)
            {
                Console.WriteLine($"RECOGNIZED: Text={recognitionResult.Text}");
                Console.WriteLine($"    Intent not recognized.");
            }
            else if (recognitionResult.Reason == ResultReason.NoMatch)
            {
                Console.WriteLine($"NOMATCH: Speech could not be recognized.");
            }
            else if (recognitionResult.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(recognitionResult);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }
            }
        }
    }
}
