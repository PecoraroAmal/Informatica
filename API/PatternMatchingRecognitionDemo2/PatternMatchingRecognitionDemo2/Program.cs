using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Intent;
using HttpProxyControl;

namespace PatternMatchingRecognitionDemo2
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
        // https://learn.microsoft.com/en-us/azure/ai-services/speech-service/how-to-use-custom-entity-pattern-matching?pivots=programming-language-csharp
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
            //IntentPatternMatchingWithMicrophoneAsync().Wait();
            IntentPatternMatchingCustomEntitiesWithMicrophoneAsync().Wait();
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
        private static async Task IntentPatternMatchingCustomEntitiesWithMicrophoneAsync()
        {
            var config = SpeechConfig.FromSubscription(azureSpeechServiceKey, azureSpeechServiceRegion);
            //imposta automaticamente il proxy se presente
            ProxyParams? proxyParams = HttpProxyHelper.GetHttpClientProxyParams();
            if (proxyParams.HasValue)
            {
                config.SetProxy(proxyParams.Value.ProxyAddress, proxyParams.Value.ProxyPort);
            }
            config.SpeechRecognitionLanguage = "it-IT"; // For example, "de-DE"

            using (var recognizer = new IntentRecognizer(config))
            {
                // Creates a Pattern Matching model and adds specific intents from your model. The
                // Id is used to identify this model from others in the collection.
                var model = new PatternMatchingModel("MyBasicPatternMatchingId");

                // Creates a pattern that uses groups of optional words. "[Go | Take me]" will match either "Go", "Take me", or "".
                var patternWithOptionalWords = "[Vai | Portami] al [piano|livello] {floorName}";

                // Creates a pattern that uses an optional entity and group that could be used to tie commands together.
                var patternWithOptionalEntity = "Vai al parcheggio [{parkingLevel}]";

                // You can also have multiple entities of the same name in a single pattern by adding appending a unique identifier
                // to distinguish between the instances. For example:
                var patternWithTwoOfTheSameEntity = "[Vai | Portami] al piano {floorName:1} [e poi al piano {floorName:2}]";
                // NOTE: Both floorName:1 and floorName:2 are tied to the same list of entries. The identifier can be a string
                //       and is separated from the entity name by a ':'

                // Adds some intents to look for specific patterns.
                model.Intents.Add(new PatternMatchingIntent("ChangeFloors", patternWithOptionalWords, patternWithOptionalEntity, patternWithTwoOfTheSameEntity));
                model.Intents.Add(new PatternMatchingIntent("DoorControl", "{action} le porte", "{action} porte", "{action} la porta", "{action} porta"));

                // Creates the "floorName" entity and set it to type list.
                // Adds acceptable values. NOTE the default entity type is Any and so we do not need
                // to declare the "action" entity.
                model.Entities.Add(PatternMatchingEntity.CreateListEntity("floorName", EntityMatchMode.Strict, "piano terra", "ingresso", "primo piano", "primo", "uno", "1", "secondo piano", "secondo", "due", "2"));

                // Creates the "parkingLevel" entity as a pre-built integer
                model.Entities.Add(PatternMatchingEntity.CreateIntegerEntity("parkingLevel"));

                var modelCollection = new LanguageUnderstandingModelCollection
                {
                    model
                };

                recognizer.ApplyLanguageModels(modelCollection);

                Console.WriteLine("Di' qualcosa...");

                var result = await recognizer.RecognizeOnceAsync();

                if (result.Reason == ResultReason.RecognizedIntent)
                {
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    Console.WriteLine($"       Intent Id={result.IntentId}.");

                    var entities = result.Entities;
                    switch (result.IntentId)
                    {
                        case "ChangeFloors":
                            if (entities.TryGetValue("floorName", out string? floorName))
                            {
                                Console.WriteLine($"       FloorName={floorName}");
                            }

                            if (entities.TryGetValue("floorName:1", out floorName))
                            {
                                Console.WriteLine($"     FloorName:1={floorName}");
                            }

                            if (entities.TryGetValue("floorName:2", out floorName))
                            {
                                Console.WriteLine($"     FloorName:2={floorName}");
                            }

                            if (entities.TryGetValue("parkingLevel", out string? parkingLevel))
                            {
                                Console.WriteLine($"    ParkingLevel={parkingLevel}");
                            }

                            break;

                        case "DoorControl":
                            if (entities.TryGetValue("action", out string? action))
                            {
                                Console.WriteLine($"          Action={action}");
                            }
                            break;
                    }
                }
                else if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    Console.WriteLine($"    Intent not recognized.");
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                    }
                }
            }
        }
    }
}
