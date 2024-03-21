using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Net;

namespace VedereAChePunto
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

        static long bytesRecieved = 0;
        static long? totalBytes;
        static int left;
        static int top;
        /// <summary>
        /// Scrive un file nel percorso di destinazione a partire da uno stream di input
        /// </summary>
        /// <param name="filePathDestination"></param>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        static async Task WriteBinaryAsync(string filePathDestination, Stream inputStream, int writeBufferSize = 4096)
        {
            using FileStream outputStream = new(filePathDestination, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: writeBufferSize, useAsync: true);

            await inputStream.CopyToAsync(outputStream);

        }

        /// <summary>
        /// Scrive un file nel percorso destinazione a partire da uno stream di input.
        /// Questa versione stampa a console la percentuale di progress.
        /// Utilizza le variabili totalBytes e butesReceived
        /// </summary>
        /// <param name="filePathDestination"></param>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        static async Task WriteBinaryAsyncWithProgress(string filePathDestination, Stream inputStream)
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.CursorVisible = false;
            }
            using FileStream outputStream = new(filePathDestination,
            FileMode.Create, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true);

            byte[] buffer = new byte[0x100000];//circa 1MB di buffer
            int numRead;
            while ((numRead = await inputStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                await outputStream.WriteAsync(buffer, 0, numRead);
                bytesRecieved += numRead;
                double? percentComplete = (double)bytesRecieved / totalBytes;
                Console.SetCursorPosition(left, top);
                Console.WriteLine($"download al {percentComplete * 100:F2}%");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {

                Console.CursorVisible = true;
            }
        }

        /// <summary>
        /// Recupera il nome del file dall'url
        /// https://stackoverflow.com/a/40361205
        /// https://stackoverflow.com/questions/1105593/get-file-name-from-uri-string-in-c-sharp
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static string GetFileNameFromUrl(string url)
        {
            Uri SomeBaseUri = new("http://canbeanything");
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
                uri = new Uri(SomeBaseUri, url);
            //Path.GetFileName funziona se ha in input un URL assoluto
            return Path.GetFileName(uri.LocalPath);
        }
        static async Task Main(string[] args)
        {
            //const string url = "https://github.com/tugberkugurlu/ASPNETWebAPISamples/archive/master.zip";
            const string url = "https://download.visualstudio.microsoft.com/download/pr/639f7cfa-84f8-48e8-b6c9-82634314e28f/8eb04e1b5f34df0c840c1bffa363c101/dotnet-sdk-3.1.100-win-x64.exe";
            HttpClient client = CreateHttpClient(setProxy: true);
            try
            {

                //leggo solo l'header HTTP, il resto verrà scaricato successivamente in maniera asincrona
                HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var sw = new Stopwatch();
                sw.Start();
                Console.WriteLine("Salvataggio su file in corso...");
                //ottengo il nome del file dall'url
                string fileName = GetFileNameFromUrl(url);
                //definisco il path complessivo del file
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + System.IO.Path.DirectorySeparatorChar + fileName;
                using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                {
                    //copio in modalità async il file 
                    //await WriteBinaryAsync(path, streamToReadFrom);
                    totalBytes = response.Content.Headers.ContentLength;
                    left = Console.CursorLeft;
                    top = Console.CursorTop;
                    await WriteBinaryAsyncWithProgress(path, streamToReadFrom);
                }
                long elapsedMs = sw.ElapsedMilliseconds;
                Console.WriteLine($"Salvataggio terminato...in {elapsedMs} ms");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);

            }


        }
    }
}
