using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Intent;
using HttpProxyControl;
using WikitextExtensions;
using Microsoft.CognitiveServices.Speech.Audio;
using OkStopEnabledRecognizingLoop.Models;

namespace OkStopEnabledRecognizingLoop
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
    //https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/primary-constructors
    //https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource-1
    //La classe TaskCompletionSourceManager serve a ospitare un campo di tipo TaskCompletionSource che va utilizzato in combinazione con i metodi:
    //ConfigureContinuousIntentPatternMatchingWithMicrophoneAsync 
    //ContinuousIntentPatternMatchingWithMicrophoneAsync
    //la classe TaskCompletionSourceManager implementa un Primary Constructor (C# ver. 12 e succ. .NET SDK 8.0 e succ.)
    public class TaskCompletionSourceManager<T>(TaskCompletionSource<T> taskCompletionSource)
    {
        TaskCompletionSource<T> _taskCompletionSource = taskCompletionSource;
        public TaskCompletionSourceManager() : this(new TaskCompletionSource<T>()) { }
        public TaskCompletionSource<T> TaskCompletionSource { get => _taskCompletionSource; set => _taskCompletionSource = value; }
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
        static AzureIntentRecognitionByCLUStore GetCLUDataFromStore()
        {
            /*
           il file è nella cartella che contiene la soluzione corrente
           il file contiene un oggetto JSON del tipo:
            {
                "api_key": "valore_chiave",
                "location_region":"valore_region",
                "language_endpoint": "valore_endpoint"
            }
            */
            string keyStorePath = "../../../../../../../MyAzureVoiceActor.json";
            string store = File.ReadAllText(keyStorePath);
            AzureIntentRecognitionByCLUStore? azureSpeechServiceStore = JsonSerializer.Deserialize<AzureIntentRecognitionByCLUStore>(store);
            return azureSpeechServiceStore ?? new AzureIntentRecognitionByCLUStore();
        }
        static AzureSpeechServiceStore GetSpeechDataFromStore()
        {
            /*
           il file è nella cartella che contiene la soluzione corrente
           il file contiene un oggetto JSON del tipo:
           {
              "api_key": "api_key_value",
              "location_region": "location_region_value",
              "endpoint": "endpoint_value"
           }
           */
            string keyStorePath = "../../../../../../../Test1.json";
            string store = File.ReadAllText(keyStorePath);
            AzureSpeechServiceStore? azureSpeechServiceStore = JsonSerializer.Deserialize<AzureSpeechServiceStore>(store);
            return azureSpeechServiceStore ?? new AzureSpeechServiceStore();
        }

        static async Task Main(string[] args)
        {
            //creazione della configurazione di SpeechConfig
            var speechConfig = SpeechConfig.FromSubscription(azureSpeechServiceKey, azureSpeechServiceRegion);
            //speechConfig.SpeechRecognitionLanguage = "en-US";
            speechConfig.SpeechRecognitionLanguage = "it-IT";
            ProxyParams? proxyParams = HttpProxyHelper.GetHttpClientProxyParams();
            if (proxyParams.HasValue)
            {
                speechConfig.SetProxy(proxyParams.Value.ProxyAddress, proxyParams.Value.ProxyPort);
            }

            //creazione del CLU Model
            var cluModel = new ConversationalLanguageUnderstandingModel(
                azureCluLanguageServiceKey,
                azureCluLanguageServiceEndpoint,
                cluProjectName,
                cluDeploymentName);

            //creazione del modello di Pattern Matching
            // Creates a Pattern Matching patternMatchingModel and adds specific intents from your patternMatchingModel. The
            // Id is used to identify this patternMatchingModel from others in the collection.
            var patternMatchingModel = new PatternMatchingModel("MyBasicPatternMatchingId");
            // Creates a pattern that uses groups of optional words.
            //https://learn.microsoft.com/en-us/azure/ai-services/speech-service/pattern-matching-overview
            //tra parentesi {} ci sono le Any Entity --> catturano qualsiasi cosa
            //https://learn.microsoft.com/en-us/azure/ai-services/speech-service/pattern-matching-overview#grouping-required-and-optional-items
            //tra parentesi [] ci sono le entity opzionali. Con il simbolo | si fa la OR. Ad esempio "[Go | Take me]" will match either "Go", "Take me", or "".
            //tra parentesi () ci sono le entity obbligatorie. Con il simbolo | si fa la OR
            var patternWithOptionalAndMandatoryWordsOk = "({startCommand}) [,] Alice [{anything}]";
            var patternWithOptionalAndMandatoryWordsStop = "[{anything2}] Alice [,] ({stopCommand})";
            var patternNone = "[{anything3}] Alice [{anything4}]";
            //intent recognition con custom entity pattern matching
            //https://learn.microsoft.com/en-us/azure/ai-services/speech-service/how-to-use-custom-entity-pattern-matching
            patternMatchingModel.Entities.Add(PatternMatchingEntity.CreateListEntity("startCommand", EntityMatchMode.Basic, "ok", "ehi", "ah", "dai", "vai"));
            patternMatchingModel.Entities.Add(PatternMatchingEntity.CreateListEntity("stopCommand", EntityMatchMode.Basic, "stop", "basta", "fermati"));
            //configurazione degli intent per il Pattern Matching
            //"ok Alice" ascolta per far partire una ricerca; "Alice stop" per fermare lo speech in corso
            //lo speech in corso è fermato sia quando viene riconosciuto l'intent di stop,
            //sia quando viene riconosciuta la frase "stop" nel testo pronunciato
            //Infatti, quando c'è il text to speech attivo, potrebbe succedere che il pattern matching non 
            //riesca a riconoscere l'intent di stop. In questo caso interviene la funzione di speech to text in loop
            //continuo che trascrive tutto quello che viene detto
            patternMatchingModel.Intents.Add(new PatternMatchingIntent("Ok", patternWithOptionalAndMandatoryWordsOk));
            patternMatchingModel.Intents.Add(new PatternMatchingIntent("Stop", patternWithOptionalAndMandatoryWordsStop));
            patternMatchingModel.Intents.Add(new PatternMatchingIntent("None", patternNone));
            var patternMatchingModelCollection = new LanguageUnderstandingModelCollection
                {
                    patternMatchingModel
                };
            var taskCompleteSourceManager = new TaskCompletionSourceManager<int>();

            //creazione di intentRecognizerByPatternMatching, speechSynthesizer e intentRecognizerByCLU
            (IntentRecognizer intentRecognizerByPatternMatching, SpeechSynthesizer speechSynthesizer, IntentRecognizer intentRecognizerByCLU) =
                ConfigureContinuousIntentPatternMatchingWithMicrophoneAsync(speechConfig, cluModel, patternMatchingModelCollection, taskCompleteSourceManager);

            //OPZIONALE: Simula l'interruzione del servizio dopo 30 secondi dall'avvio

            //_ = Task.Factory.StartNew(async () =>
            //{
            //    await Task.Delay(30000);
            //    await intentRecognizerByPatternMatching.StopContinuousRecognitionAsync();
            //    await speechSynthesizer.StopSpeakingAsync();
            //    //il Dispose delle risorse serve quando non si usa la clausola using
            //    //il Dispose richiede che il sintetizatore non sia in funzione
            //    //speechSynthesizer.Dispose();
            //    //intentRecognizerByPatternMatching.Dispose();
            //    //intentRecognizerByCLU.Dispose();
            //});

            //quando parte la chiamata a ContinuousIntentPatternMatchingWithMicrophoneAsync si rimane bloccati fino a che il servizio non viene interrotto
            await ContinuousIntentPatternMatchingWithMicrophoneAsync(intentRecognizerByPatternMatching, taskCompleteSourceManager).ConfigureAwait(false);

            // OPZIONALE: quando passa il tempo previsto dal task di interruzione il servizio di recognition e lo speech vengono interrotti
            //simuliamo la ripresa del servizio senza ricreare l'oggetto recognizerByCLU o syntethizer

            //await Task.Delay(1000);
            //Console.WriteLine("Riparte il servizio");
            ////prepariamo un nuovo TaskCompletionSource per fare in modo di aspettare fino a che il continous recognition non venga nuovamente interrotto
            //taskCompleteSourceManager.TaskCompletionSource = new TaskCompletionSource<int>();
            //await ContinuousIntentPatternMatchingWithMicrophoneAsync(intentRecognizerByPatternMatching, taskCompleteSourceManager).ConfigureAwait(false);

        }

        private static async Task ContinuousIntentPatternMatchingWithMicrophoneAsync(IntentRecognizer intentRecognizer, TaskCompletionSourceManager<int> stopRecognition)
        {
            const string fraseOK = "ok Alice";
            const string fraseStop = "Alice stop";
            Console.WriteLine($"In ascolto. Dì \"{fraseOK}\" per impartire un ordine, oppure \"{fraseStop}\" per fermare l'azione in corso");
            await intentRecognizer.StartContinuousRecognitionAsync();
            // Waits for completion. Use Task.WaitAny to keep the task rooted.
            Task.WaitAny(new[] { stopRecognition.TaskCompletionSource.Task });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="cluModel"></param>
        /// <param name="patternMatchingModelCollection"></param>
        /// <param name="stopRecognitionManager"></param>
        /// <returns>una tupla contentente nell'ordine un intent recognizer basato su Patter Matching, un sintetizzatore vocale e un intent recognizer basato su un modello di Conversational Language Understanding </returns>
        private static (IntentRecognizer, SpeechSynthesizer, IntentRecognizer) ConfigureContinuousIntentPatternMatchingWithMicrophoneAsync(
            SpeechConfig config,
            ConversationalLanguageUnderstandingModel cluModel,
            LanguageUnderstandingModelCollection patternMatchingModelCollection,
            TaskCompletionSourceManager<int> stopRecognitionManager)
        {
            //creazione di un intent recognizer basato su pattern matching
            var intentRecognizerByPatternMatching = new IntentRecognizer(config);
            intentRecognizerByPatternMatching.ApplyLanguageModels(patternMatchingModelCollection);

            //creazione di un intent recognizer basato su CLU
            var intentRecognizerByCLU = new IntentRecognizer(config);
            var modelsCollection = new LanguageUnderstandingModelCollection { cluModel };
            intentRecognizerByCLU.ApplyLanguageModels(modelsCollection);

            //creazione di un sitetizzatore vocale
            var synthesizer = new SpeechSynthesizer(config);

            //gestione eventi
            intentRecognizerByPatternMatching.Recognized += async (s, e) =>
            {
                switch (e.Result.Reason)
                {
                    case ResultReason.RecognizedSpeech:
                        Console.WriteLine($"PATTERN MATCHING - RECOGNIZED SPEECH: Text= {e.Result.Text}");
                        break;
                    case ResultReason.RecognizedIntent:
                        {
                            Console.WriteLine($"PATTERN MATCHING - RECOGNIZED INTENT: Text= {e.Result.Text}");
                            Console.WriteLine($"       Intent Id= {e.Result.IntentId}.");
                            if (e.Result.IntentId == "Ok")
                            {
                                Console.WriteLine("Stopping current speaking if any...");
                                await synthesizer.StopSpeakingAsync();
                                await HandleOkCommand(synthesizer, intentRecognizerByCLU);
                            }
                            else if (e.Result.IntentId == "Stop")
                            {
                                Console.WriteLine("Stopping current speaking...");
                                await synthesizer.StopSpeakingAsync();
                            }
                        }

                        break;
                    case ResultReason.NoMatch:
                        Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                        var noMatch = NoMatchDetails.FromResult(e.Result);
                        switch (noMatch.Reason)
                        {
                            case NoMatchReason.NotRecognized:
                                Console.WriteLine($"PATTERN MATCHING - NOMATCH: Speech was detected, but not recognized.");
                                break;
                            case NoMatchReason.InitialSilenceTimeout:
                                Console.WriteLine($"PATTERN MATCHING - NOMATCH: The start of the audio stream contains only silence, and the service timed out waiting for speech.");
                                break;
                            case NoMatchReason.InitialBabbleTimeout:
                                Console.WriteLine($"PATTERN MATCHING - NOMATCH: The start of the audio stream contains only noise, and the service timed out waiting for speech.");
                                break;
                            case NoMatchReason.KeywordNotRecognized:
                                Console.WriteLine($"PATTERN MATCHING - NOMATCH: Keyword not recognized");
                                break;
                        }
                        break;

                    default:
                        break;
                }
            };
            intentRecognizerByPatternMatching.Canceled += (s, e) =>
            {
                Console.WriteLine($"PATTERN MATCHING - CANCELED: Reason={e.Reason}");

                if (e.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"PATTERN MATCHING - CANCELED: ErrorCode={e.ErrorCode}");
                    Console.WriteLine($"PATTERN MATCHING - CANCELED: ErrorDetails={e.ErrorDetails}");
                    Console.WriteLine($"PATTERN MATCHING - CANCELED: Did you update the speech key and location/region info?");
                }
                stopRecognitionManager.TaskCompletionSource.TrySetResult(0);
            };
            intentRecognizerByPatternMatching.SessionStopped += (s, e) =>
            {
                Console.WriteLine("\n    Session stopped event.");
                stopRecognitionManager.TaskCompletionSource.TrySetResult(0);
            };

            return (intentRecognizerByPatternMatching, synthesizer, intentRecognizerByCLU);

        }
        private static async Task HandleOkCommand(SpeechSynthesizer synthesizer, IntentRecognizer intentRecognizer)
        {
            await synthesizer.SpeakTextAsync("Sono in ascolto");
            //avvia l'intent recognition su Azure
            string? jsonResult = await RecognizeIntentAsync(intentRecognizer);
            if (jsonResult != null)
            {

                //process jsonResult
                //deserializzo il json
                JsonSerializerOptions options = new(JsonSerializerDefaults.Web) { WriteIndented = true };
                CLUResponse cluResponse = JsonSerializer.Deserialize<CLUResponse>(jsonResult, options) ?? new CLUResponse();
                await synthesizer.SpeakTextAsync($"La tua richiesta è stata {cluResponse.Result?.Query}");
                var topIntent = cluResponse.Result?.Prediction?.TopIntent;

                if (topIntent != null)
                {
                    switch (topIntent)
                    {
                        case string intent when intent.Contains("Wiki"):
                            await synthesizer.SpeakTextAsync("Vuoi fare una ricerca su Wikipedia");
                            break;
                        case string intent when intent.Contains("Weather"):
                            await synthesizer.SpeakTextAsync("Vuoi sapere come è il tempo");
                            break;
                        case string intent when intent.Contains("Places"):
                            await synthesizer.SpeakTextAsync("Vuoi informazioni geolocalizzate");
                            break;

                    }

                }
                //determino l'action da fare, eventualmente effettuando una richiesta GET su un endpoint remoto scelto in base al topScoringIntent
                //ottengo il risultato dall'endpoit remoto
                //effettuo un text to speech per descrivere il risultato
            }
            else
            {
                //non è stato riconosciuto l'intent
                await synthesizer.SpeakTextAsync("Intent non riconosciuto");
            }


        }

        public static async Task<string?> RecognizeIntentAsync(IntentRecognizer recognizer)
        {
            // Starts recognizing.
            Console.WriteLine("Say something...");

            // Starts intent recognition, and returns after a single utterance is recognized. The end of a
            // single utterance is determined by listening for silence at the end or until a maximum of 15
            // seconds of audio is processed.  The task returns the recognition text as result. 
            // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
            // shot recognition like command or query. 
            // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
            var result = await recognizer.RecognizeOnceAsync();
            string? languageUnderstandingJSON = null;
            // Checks result.
            switch (result.Reason)
            {
                case ResultReason.RecognizedIntent:
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    Console.WriteLine($"    Intent Id: {result.IntentId}.");
                    languageUnderstandingJSON = result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult);
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
            return languageUnderstandingJSON;
        }
    }
}
