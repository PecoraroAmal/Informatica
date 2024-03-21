using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Net;

namespace HttpClientWebPageDownload
{
    class Program
    {
        /// <summary>
        /// Effettua il setup del client Http con l'eventuale proxy (se presente)
        /// Richiede: 
        /// 1) l'istallazione del pacchetto Microsoft.Win32.Registry tramite NuGet
        /// 2) using Microsoft.Win32;
        /// 3) using System.Runtime.InteropServices;
        /// 4) using System.Net;
        /// </summary>
        /// <param name="setProxy"></param>
        /// <returns>un oggetto HttpClient con eventuale proxy configurato</returns>
        public static HttpClient CreateHttpClient(bool setProxy)
        {
            if (setProxy)
            {
                Uri? proxy;
                //https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.defaultproxy?view=net-6.0
                //https://medium.com/@sddkal/net-core-interaction-with-registry-4d7fcabc7a6b
                //https://www.shellhacks.com/windows-show-proxy-settings-cmd-powershell/
                //https://stackoverflow.com/a/63884955

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    //ottengo lo user specific proxy che si ottiene con il comando:
                    //C:\> reg query "HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings"
                    //per poter utilizzare Registry occorre:
                    //1) --> istallare il pacchetto Microsoft.Win32.Registry tramite NuGet
                    //2) --> using Microsoft.Win32;

                    //leggiamo lo user specific proxy direttamente dal registro di sistema di Windows
                    RegistryKey? internetSettings = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings");
                    //il proxy viene abilitato mediante il valore della chiave di registro ProxyEnable
                    int? proxyEnable = internetSettings?.GetValue("ProxyEnable") as int?;
                    //impostiamo proxy
                    proxy = (proxyEnable > 0 && internetSettings?.GetValue("ProxyServer") is string userProxy) ? new Uri(userProxy) : null;

                }
                else //se il sistema operativo è diverso da Windows procediamo con la determinazione del system wide proxy (se impostato)
                {
                    //questa è la procedura per ottenere il system proxy
                    Uri destinationUri = new("https://www.google.it");
                    //Ottiene il default proxy quando si prova a contattare la destinationUri
                    //Se il proxy non è impostato si ottiene null
                    //Uri proxy = HttpClient.DefaultProxy.GetProxy(destinationUri);
                    //Con il proxy calcolato in automatico si crea l'handler da passare all'oggetto HttpClient e
                    //funziona sia che il proxy sia configurato sia che non lo sia
                    proxy = HttpClient.DefaultProxy.GetProxy(destinationUri);
                }

                //con il proxy ottenuto con il codice precedente
                HttpClientHandler httpHandler = new()
                {
                    Proxy = new WebProxy(proxy, true),
                    UseProxy = true,
                    PreAuthenticate = false,
                    UseDefaultCredentials = false,
                };
                return new HttpClient(httpHandler);
            }
            else
            {
                return new HttpClient();
            }
        }
        static async Task Main(string[] args)
        {
            HttpClient client = CreateHttpClient(setProxy: true);
            try
            {
                string responseBody = await client.GetStringAsync("https://www.zerochan.net/Genshin+Impact?q=Genshin+Impact");
                //salvataggio su file
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + System.IO.Path.DirectorySeparatorChar + "pagina.html";
                File.WriteAllText(path, responseBody);
                Console.WriteLine("File scaricato, controlla la cartella Desktop");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);

            }
        }
    }
}
