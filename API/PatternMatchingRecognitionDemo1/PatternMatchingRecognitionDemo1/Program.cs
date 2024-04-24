using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Intent;
using HttpProxyControl;

namespace PatternMatchingRecognitionDemo1
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
    class Program
    {
        //https://learn.microsoft.com/en-us/azure/ai-services/speech-service/how-to-use-simple-language-pattern-matching?pivots=programming-language-csharp
        static readonly AzureSpeechServiceStore azureSpeechServiceStore = GetDataFromStore();
        static readonly string azureSpeechServiceKey = azureSpeechServiceStore.APIKeyValue;
        static readonly string azureSpeechServiceRegion = azureSpeechServiceStore.LocationRegion;

        static AzureSpeechServiceStore GetDataFromStore()
        {
            //il file è nella cartella che contiene la soluzione corrente
            //il file contiene un oggetto JSON del tipo:
            // {
            //   "api_key": "api_key_value",
            //   "location_region: "location_region_value",
            //   "endpoint": "endpoint_value"       
            //}
            string keyStorePath = "../../../../../../../MyAzureVoiceActor.json";
            string store = File.ReadAllText(keyStorePath);
            AzureSpeechServiceStore? azureSpeechServiceStore = JsonSerializer.Deserialize<AzureSpeechServiceStore>(store);
            return azureSpeechServiceStore ?? new AzureSpeechServiceStore();
        }
        static void Main(string[] args)
        {
            IntentPatternMatchingWithMicrophoneAsync().Wait();
        }
        private static async Task IntentPatternMatchingWithMicrophoneAsync()
        {
            var config = SpeechConfig.FromSubscription(azureSpeechServiceKey, azureSpeechServiceRegion);
            //imposta automaticamente il proxy se presente
            ProxyParams? proxyParams = HttpProxyHelper.GetHttpClientProxyParams();
            if (proxyParams.HasValue)
            {
                config.SetProxy(proxyParams.Value.ProxyAddress, proxyParams.Value.ProxyPort);
            }
            config.SpeechRecognitionLanguage = "it-IT"; // For example, "de-DE"
            using (var intentRecognizer = new IntentRecognizer(config))
            {
                intentRecognizer.AddIntent("Portami al piano {floorName}.", "ChangeFloors");
                intentRecognizer.AddIntent("Vai al piano {floorName}.", "ChangeFloors");
                intentRecognizer.AddIntent("{action} la porta.", "OpenCloseDoor");

                Console.WriteLine("Say something...");

                var result = await intentRecognizer.RecognizeOnceAsync();

                switch (result.Reason)
                {
                    case ResultReason.RecognizedSpeech:
                        Console.WriteLine($"RECOGNIZED: Text= {result.Text}");
                        Console.WriteLine($"    Intent not recognized.");
                        break;
                    case ResultReason.RecognizedIntent:
                        Console.WriteLine($"RECOGNIZED: Text= {result.Text}");
                        Console.WriteLine($"       Intent Id= {result.IntentId}.");
                        var entities = result.Entities;
                        if (entities.TryGetValue("floorName", out string? floorNameValue))
                        {
                            Console.WriteLine($"       FloorName= {floorNameValue}");
                        }

                        if (entities.TryGetValue("action", out string? actionValue))
                        {
                            Console.WriteLine($"       Action= {actionValue}");
                        }

                        break;
                    case ResultReason.NoMatch:
                        Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                        var noMatch = NoMatchDetails.FromResult(result);
                        switch (noMatch.Reason)
                        {
                            case NoMatchReason.NotRecognized:
                                Console.WriteLine($"NOMATCH: Speech was detected, but not recognized.");
                                break;
                            case NoMatchReason.InitialSilenceTimeout:
                                Console.WriteLine($"NOMATCH: The start of the audio stream contains only silence, and the service timed out waiting for speech.");
                                break;
                            case NoMatchReason.InitialBabbleTimeout:
                                Console.WriteLine($"NOMATCH: The start of the audio stream contains only noise, and the service timed out waiting for speech.");
                                break;
                            case NoMatchReason.KeywordNotRecognized:
                                Console.WriteLine($"NOMATCH: Keyword not recognized");
                                break;
                        }
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
                    default:
                        break;
                }
            }
        }
    }
}
