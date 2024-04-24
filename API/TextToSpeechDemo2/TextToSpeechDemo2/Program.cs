using Microsoft.CognitiveServices.Speech;
using HttpProxyControl;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.CognitiveServices.Speech.Audio;

namespace TextToSpeechDemo2
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

        async static Task Main(string[] args)
        {
            string testo = @"Il Filocolo (secondo un'etimologia approssimativa «fatica d'amore») è un romanzo in prosa: rappresenta una svolta rispetto ai romanzi delle origini scritti in versi. La storia ha come protagonisti Florio, figlio di un re saraceno, e Biancifiore (o Biancofiore), una schiava cristiana abbandonata da bambina. I due fanciulli crescono assieme e da grandi, in seguito alla lettura del libro di Ovidio Ars Amandi s'innamorano, come era successo per Paolo e Francesca dopo avere letto Ginevra e Lancillotto. Tuttavia il padre di Florio decide di separarli vendendo Biancifiore a dei mercanti. Florio decide quindi di andarla a cercare e dopo mille peripezie (da qui il titolo Filocolo) la rincontra. Infine, il giovane si converte al cristianesimo e sposa la fanciulla.";
            await EsempioTextToSpeechAudioOutput(testo);
            //await EsempioTextToSpeechFileOutput(testo, "Filocolo.wav");
            //await EsempioTextToSpeechStreamOutput(testo, "Filocolo2.wav");

        }

        private static async Task EsempioTextToSpeechAudioOutput(string text)
        {
            var speechConfig = SpeechConfig.FromSubscription(azureSpeechServiceKey, azureSpeechServiceRegion);
            //imposta automaticamente il proxy se presente
            ProxyParams? proxyParams = HttpProxyHelper.GetHttpClientProxyParams();
            if (proxyParams.HasValue)
            {
                speechConfig.SetProxy(proxyParams.Value.ProxyAddress, proxyParams.Value.ProxyPort);
            }

            // The neural multilingual voice can speak different languages based on the input text.
            speechConfig.SpeechSynthesisVoiceName = "en-US-AndrewMultilingualNeural";
            //or just use local voice
            //speechConfig.SpeechSynthesisVoiceName = "it-IT-ElsaNeural";
            //speechConfig.SpeechSynthesisVoiceName = "it-IT-GianniNeural";
            using var synthesizer = new SpeechSynthesizer(speechConfig);
            //await synthesizer.SpeakTextAsync("A simple test to write to a file.");
            await synthesizer.SpeakTextAsync(text);
        }

        private static async Task EsempioTextToSpeechFileOutput(string text, string outputFileName)
        {
            var speechConfig = SpeechConfig.FromSubscription(azureSpeechServiceKey, azureSpeechServiceRegion);
            //imposta automaticamente il proxy se presente
            ProxyParams? proxyParams = HttpProxyHelper.GetHttpClientProxyParams();
            if (proxyParams.HasValue)
            {
                speechConfig.SetProxy(proxyParams.Value.ProxyAddress, proxyParams.Value.ProxyPort);
            }

            // The neural multilingual voice can speak different languages based on the input text.
            speechConfig.SpeechSynthesisVoiceName = "en-US-AndrewMultilingualNeural";
            //or just use local voice
            //speechConfig.SpeechSynthesisVoiceName = "it-IT-ElsaNeural";
            //speechConfig.SpeechSynthesisVoiceName = "it-IT-GianniNeural";
            using var audioConfig = AudioConfig.FromWavFileOutput(outputFileName);
            using var synthesizer = new SpeechSynthesizer(speechConfig, audioConfig);
            //await synthesizer.SpeakTextAsync("A simple test to write to a file.");
            await synthesizer.SpeakTextAsync(text);
        }

        static async Task EsempioTextToSpeechStreamOutput(string text, string outputFileName)
        {
            var speechConfig = SpeechConfig.FromSubscription(azureSpeechServiceKey, azureSpeechServiceRegion);
            //imposta automaticamente il proxy se presente
            ProxyParams? proxyParams = HttpProxyHelper.GetHttpClientProxyParams();
            if (proxyParams.HasValue)
            {
                speechConfig.SetProxy(proxyParams.Value.ProxyAddress, proxyParams.Value.ProxyPort);
            }

            // The neural multilingual voice can speak different languages based on the input text.
            speechConfig.SpeechSynthesisVoiceName = "en-US-AndrewMultilingualNeural";
            //or just use local voice
            //speechConfig.SpeechSynthesisVoiceName = "it-IT-ElsaNeural";
            //speechConfig.SpeechSynthesisVoiceName = "it-IT-GianniNeural";
            speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);
            using var synthesizer = new SpeechSynthesizer(speechConfig, null);
            var result = await synthesizer.SpeakTextAsync(text);
            using var stream = AudioDataStream.FromResult(result);
            await stream.SaveToWaveFileAsync(outputFileName);
        }

    }
}
