using System.Text.Json;
using System.Text.Json.Serialization;
using HttpProxyControl;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
//Esempio tratto da
//https://learn.microsoft.com/en-us/azure/ai-services/speech-service/get-started-speech-to-text
//con alcune importanti modifiche
namespace SpeechRecognitionDemo1
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
        static readonly AzureSpeechServiceStore azureSpeechServiceStore = GetDataFromStore();
        static readonly string azureSpeechServiceKey = azureSpeechServiceStore.APIKeyValue;
        static readonly string azureSpeechServiceRegion = azureSpeechServiceStore.LocationRegion;
        static readonly bool languageAutoDetect = true;
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

        // This example requires environment variables named "SPEECH_KEY" and "SPEECH_REGION"
        //static string speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
        //static string speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");

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
            var speechConfig = SpeechConfig.FromSubscription(azureSpeechServiceKey, azureSpeechServiceRegion);
            using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            //imposta automaticamente il proxy se presente
            ProxyParams? proxyParams = HttpProxyHelper.GetHttpClientProxyParams();
            if (proxyParams.HasValue)
            {
                speechConfig.SetProxy(proxyParams.Value.ProxyAddress, proxyParams.Value.ProxyPort);
            }
            //se si vuole il riconoscimento automatico della lingua allo startup del servizio - impiega più tempo per riconoscere il testo
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

        }
    }
}
