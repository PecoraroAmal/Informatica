using HttpProxyControl;

namespace Meteo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = HttpProxyHelper.CreateHttpClient(setProxy: true);
            FormattableString formattableString = $"";
            string stringa = FormattableString.Invariant(formattableString);
            var richiesta = await client.GetAsync($"{stringa}");
            if(richiesta.IsSuccessStatusCode)
            {
                //OpenMeteoForecast? forecast = await.richiesta.Content.ReadFromJsonAsync<OpenMeteoForecast>();
            }
        }
    }
}
