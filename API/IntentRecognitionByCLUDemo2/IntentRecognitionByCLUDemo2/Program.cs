using HttpProxyControl;
using IntentRecognitionByCLUDemo2.Models;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Intent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntentRecognitionByCLUDemo2
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
    internal class Program
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

        //https://learn.microsoft.com/en-us/azure/ai-services/speech-service/how-to-recognize-intents-from-speech-csharp#continuous-recognition-from-a-file
        //con modifiche per adattare l'esempio al ConversationalLanguageUnderstandingModel
        public static async Task ContinuousRecognitionIntentFromFileAsync(SpeechConfig config, ConversationalLanguageUnderstandingModel model, string inputFile)
        {
            //https://learn.microsoft.com/en-us/azure/ai-services/speech-service/how-to-recognize-intents-from-speech-csharp#continuous-recognition-from-a-file
            using var audioInput = AudioConfig.FromWavFileInput(inputFile);
            using var recognizer = new IntentRecognizer(config, audioInput);
            // The TaskCompletionSource to stop recognition.
            var stopRecognition = new TaskCompletionSource<int>();

            var modelsCollection = new LanguageUnderstandingModelCollection { model };
            recognizer.ApplyLanguageModels(modelsCollection);

            // Subscribes to events.
            recognizer.Recognizing += (s, e) =>
            {
                Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
            };

            recognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedIntent)
                {
                    Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                    Console.WriteLine($"    Intent Id: {e.Result.IntentId}.");
                    Console.WriteLine($"    Language Understanding JSON: {e.Result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult)}.");
                }
                else if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                    Console.WriteLine($"    Intent not recognized.");
                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
            };

            recognizer.Canceled += (s, e) =>
            {
                Console.WriteLine($"CANCELED: Reason={e.Reason}");

                if (e.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }

                stopRecognition.TrySetResult(0);
            };

            recognizer.SessionStarted += (s, e) =>
            {
                Console.WriteLine("\n    Session started event.");
            };

            recognizer.SessionStopped += (s, e) =>
            {
                Console.WriteLine("\n    Session stopped event.");
                Console.WriteLine("\nStop recognition.");
                stopRecognition.TrySetResult(0);
            };

            // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
            Console.WriteLine("Say something...");
            //why we use ConfigureAwait(false)
            //https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca2007
            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

            // Waits for completion.
            // Use Task.WaitAny to keep the task rooted.
            Task.WaitAny(new[] { stopRecognition.Task });

            // Stops recognition.
            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
        }
        public static async Task ContinuousRecognitionIntentAsync(SpeechConfig config, ConversationalLanguageUnderstandingModel model)
        {
            using var audioInput = AudioConfig.FromDefaultMicrophoneInput();
            using var recognizer = new IntentRecognizer(config, audioInput);
            // The TaskCompletionSource to stop recognition.
            var stopRecognition = new TaskCompletionSource<int>();

            var modelsCollection = new LanguageUnderstandingModelCollection { model };
            recognizer.ApplyLanguageModels(modelsCollection);

            // Subscribes to events.
            recognizer.Recognizing += (s, e) =>
            {
                Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
            };

            recognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedIntent)
                {
                    Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                    Console.WriteLine($"    Intent Id: {e.Result.IntentId}.");
                    string languageUnderstandingJSON = e.Result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult);
                    Console.WriteLine($"    Language Understanding JSON: {languageUnderstandingJSON}.");
                    JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true };
                    CLUResponse cluResponse = JsonSerializer.Deserialize<CLUResponse>(languageUnderstandingJSON, options) ?? new CLUResponse();
                    Console.WriteLine("Risultato deserializzato:");
                    Console.WriteLine($"kind: {cluResponse.Kind}");
                    Console.WriteLine($"result.query: {cluResponse.Result?.Query}");
                    Console.WriteLine($"result.prediction.topIntent: {cluResponse.Result?.Prediction?.TopIntent}");
                    Console.WriteLine($"result.prediction.Intents[0].Category: {cluResponse.Result?.Prediction?.Intents?[0].Category}");
                    Console.WriteLine($"result.prediction.Intents[0].ConfidenceScore: {cluResponse.Result?.Prediction?.Intents?[0].ConfidenceScore}");
                    Console.WriteLine($"result.prediction.entities: ");
                    cluResponse.Result?.Prediction?.Entities?.ForEach(s => Console.WriteLine($"\tcategory = {s.Category}; text= {s.Text};"));
                    Console.WriteLine("***************************************************");
                }
                else if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                    Console.WriteLine($"    Intent not recognized.");
                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
            };

            recognizer.Canceled += (s, e) =>
            {
                Console.WriteLine($"CANCELED: Reason={e.Reason}");

                if (e.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }

                stopRecognition.TrySetResult(0);
            };

            recognizer.SessionStarted += (s, e) =>
            {
                Console.WriteLine("\n    Session started event.");
            };

            recognizer.SessionStopped += (s, e) =>
            {
                Console.WriteLine("\n    Session stopped event.");
                Console.WriteLine("\nStop recognition.");
                stopRecognition.TrySetResult(0);
            };

            // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
            Console.WriteLine("Say something...");
            //why we use ConfigureAwait(false)
            //https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca2007
            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

            // Waits for completion.
            // Use Task.WaitAny to keep the task rooted.
            Task.WaitAny(new[] { stopRecognition.Task });

            // Stops recognition.
            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
        }
        public static async Task RecognizeIntentAsync(SpeechConfig config, ConversationalLanguageUnderstandingModel model)
        {

            using var audioInput = AudioConfig.FromDefaultMicrophoneInput();
            using var recognizer = new IntentRecognizer(config, audioInput);

            var modelsCollection = new LanguageUnderstandingModelCollection { model };
            recognizer.ApplyLanguageModels(modelsCollection);

            // Starts recognizing.
            Console.WriteLine("Say something...");

            // Starts intent recognition, and returns after a single utterance is recognized. The end of a
            // single utterance is determined by listening for silence at the end or until a maximum of 15
            // seconds of audio is processed.  The task returns the recognition text as result. 
            // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
            // shot recognition like command or query. 
            // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
            var result = await recognizer.RecognizeOnceAsync();

            // Checks result.
            switch (result.Reason)
            {
                case ResultReason.RecognizedIntent:
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    Console.WriteLine($"    Intent Id: {result.IntentId}.");
                    string languageUnderstandingJSON = result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult);
                    Console.WriteLine($"    Language Understanding JSON: {languageUnderstandingJSON}.");
                    JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true };
                    CLUResponse cluResponse = JsonSerializer.Deserialize<CLUResponse>(languageUnderstandingJSON, options) ?? new CLUResponse();
                    Console.WriteLine("Risultato deserializzato:");
                    Console.WriteLine($"kind: {cluResponse.Kind}");
                    Console.WriteLine($"result.query: {cluResponse.Result?.Query}");
                    Console.WriteLine($"result.prediction.topIntent: {cluResponse.Result?.Prediction?.TopIntent}");
                    Console.WriteLine($"result.prediction.Intents[0].Category: {cluResponse.Result?.Prediction?.Intents?[0].Category}");
                    Console.WriteLine($"result.prediction.Intents[0].ConfidenceScore: {cluResponse.Result?.Prediction?.Intents?[0].ConfidenceScore}");
                    Console.WriteLine($"result.prediction.entities: ");
                    cluResponse.Result?.Prediction?.Entities?.ForEach(s => Console.WriteLine($"\tcategory = {s.Category}; text= {s.Text};"));
                    break;
                case ResultReason.RecognizedSpeech:
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    Console.WriteLine($"    Intent not recognized.");
                    break;
                case ResultReason.NoMatch:
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                    break;
                case ResultReason.Canceled:
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                    break;
            }
        }

        static async Task Main(string[] args)
        {
            var speechConfig = SpeechConfig.FromSubscription(azureSpeechServiceKey, azureSpeechServiceRegion);
            //speechConfig.SpeechRecognitionLanguage = "en-US";
            speechConfig.SpeechRecognitionLanguage = "it-IT";
            ProxyParams? proxyParams = HttpProxyHelper.GetHttpClientProxyParams();
            if (proxyParams.HasValue)
            {
                speechConfig.SetProxy(proxyParams.Value.ProxyAddress, proxyParams.Value.ProxyPort);
            }

            var cluModel = new ConversationalLanguageUnderstandingModel(
                azureCluLanguageServiceKey,
                azureCluLanguageServiceEndpoint,
                cluProjectName,
                cluDeploymentName);

            //await RecognizeIntentAsync(speechConfig, cluModel).ConfigureAwait(false);
            //Console.WriteLine("Please press <Return> to continue.");
            //Console.ReadLine();

            await ContinuousRecognitionIntentAsync(speechConfig, cluModel).ConfigureAwait(false);

        }
    }
}
