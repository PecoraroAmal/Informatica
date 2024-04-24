using System.Text.Json;
using System.Text.Json.Serialization;
using HttpProxyControl;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
namespace SpeechRecognitionDemo2
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
        static readonly bool languageAutoDetect = false;
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

        async static Task FromFile(SpeechConfig speechConfig)
        {
            using var audioConfig = AudioConfig.FromWavFileInput("PathToFile.wav");
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);
            var result = await recognizer.RecognizeOnceAsync();
            Console.WriteLine($"RECOGNIZED: Text={result.Text}");
        }
        async static Task FromStream(SpeechConfig speechConfig)
        {
            var reader = new BinaryReader(File.OpenRead("PathToFile.wav"));
            using var audioInputStream = AudioInputStream.CreatePushStream();
            using var audioConfig = AudioConfig.FromStreamInput(audioInputStream);
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);
            byte[] readBytes;
            do
            {
                readBytes = reader.ReadBytes(1024);
                audioInputStream.Write(readBytes, readBytes.Length);
            } while (readBytes.Length > 0);
            var result = await recognizer.RecognizeOnceAsync();
            Console.WriteLine($"RECOGNIZED: Text={result.Text}");
        }
        async static Task FromMic(SpeechRecognizer recognizer)
        {
            Console.WriteLine("Speak into your microphone.");
            var result = await recognizer.RecognizeOnceAsync();
            Console.WriteLine($"RECOGNIZED: Text={result.Text}");
        }

        async static Task ContinuousRecognition(SpeechRecognizer recognizer)
        {

            var stopRecognition = new TaskCompletionSource<int>();
            recognizer.Recognizing += (s, e) =>
            {
                Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
            };

            recognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
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
                    Console.WriteLine($"CANCELED: Did you update the speech key and location/region info?");
                }

                stopRecognition.TrySetResult(0);
            };

            recognizer.SessionStopped += (s, e) =>
            {
                Console.WriteLine("\n    Session stopped event.");
                stopRecognition.TrySetResult(0);
            };
            await recognizer.StartContinuousRecognitionAsync();
            // Waits for completion. Use Task.WaitAny to keep the task rooted.
            Task.WaitAny(new[] { stopRecognition.Task });

            // Make the following call at some point to stop recognition:
            // await recognizer.StopContinuousRecognitionAsync();
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
            //NAudio ****
            //
            //questa parte serve solo se si vuole selezionare una sorgente diversa dal microfono di default
            //Solo in questo caso è richiesto installare anche NAudio 
            //var enumerator = new MMDeviceEnumerator();
            //foreach (var endpoint in
            //         enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            //{
            //    Console.WriteLine("{0} Id = {1}", endpoint.FriendlyName, endpoint.ID);
            //}
            //using var audioConfig = AudioConfig.FromMicrophoneInput(enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).First().ID);
            //NAudio

            var speechConfig = SpeechConfig.FromSubscription(azureSpeechServiceKey, azureSpeechServiceRegion);

            using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            //imposta automaticamente il proxy se presente
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
                //await FromMic(speechRecognizer);
                await ContinuousRecognition(speechRecognizer);
                //come nel primo esempio
                //Console.WriteLine("Parla nel microfono.");
                //var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
                //OutputSpeechRecognitionResult(speechRecognitionResult);

            }
            else //configurazione con cultura pre-impostata
            {
                speechConfig.SpeechRecognitionLanguage = "it-IT";
                using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);
                //await FromMic(speechRecognizer);
                await ContinuousRecognition(speechRecognizer);
                //come nel primo esempio
                //Console.WriteLine("Parla nel microfono.");
                //var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
                //OutputSpeechRecognitionResult(speechRecognitionResult);

            }

        }
    }
}
