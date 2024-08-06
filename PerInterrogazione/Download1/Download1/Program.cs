using HttpProxyControl;
namespace Download1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = HttpProxyHelper.CreateHttpClient(setProxy:true);
            try
            {
                string url = "https://www.3bmeteo.com/";
                string sito = await client.GetStringAsync(url);
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "sito.html");
                File.WriteAllText(path, sito);
                await Console.Out.WriteLineAsync("Guarda il desktop");

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Errore: {ex.Message}");
            }
        }
    }
}
