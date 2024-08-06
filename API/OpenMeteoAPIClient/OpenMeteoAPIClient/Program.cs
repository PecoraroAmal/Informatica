using HttpProxyControl;
using OpenMeteoAPIClient.Model.Minimale;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

namespace OpenMeteoAPIClient
{
    internal class Program
    {
        static readonly HttpClient _client = HttpProxyHelper.CreateHttpClient(setProxy: true);
        static async Task Main(string[] args)
        {
            await EsempioPrevisioniMeteo();
        }
        static async Task EsempioPrevisioniMeteo()
        {
            Console.WriteLine("\n****************************");
            Console.WriteLine("Metodo: EsempioPrevisioniMeteo");
            Console.WriteLine("****************************\n");
            const string datoNonFornitoString = "Dato non fornito";//si potrebbe mettere una stringa vuota

            try
            {
                string place = "Besana in Brianza";
                (double? lat, double? lon)? geo = await Utils.GeocodeByOpenMeteo(_client, place);

                if (geo != null)
                {
                    //richiesta di tutto (con il timezone di Roma):
                    //https://api.open-meteo.com/v1/forecast?latitude={geo?.lat}&longitude={geo?.lon}&current=temperature_2m,relative_humidity_2m,apparent_temperature,is_day,precipitation,rain,showers,snowfall,weather_code,cloud_cover,pressure_msl,surface_pressure,wind_speed_10m,wind_direction_10m,wind_gusts_10m&hourly=temperature_2m,relative_humidity_2m,dew_point_2m,apparent_temperature,precipitation_probability,precipitation,rain,showers,snowfall,snow_depth,weather_code,pressure_msl,surface_pressure,cloud_cover,cloud_cover_low,cloud_cover_mid,cloud_cover_high,visibility,evapotranspiration,et0_fao_evapotranspiration,vapour_pressure_deficit,wind_speed_10m,wind_speed_80m,wind_speed_120m,wind_speed_180m,wind_direction_10m,wind_direction_80m,wind_direction_120m,wind_direction_180m,wind_gusts_10m,temperature_80m,temperature_120m,temperature_180m,soil_temperature_0cm,soil_temperature_6cm,soil_temperature_18cm,soil_temperature_54cm,soil_moisture_0_to_1cm,soil_moisture_1_to_3cm,soil_moisture_3_to_9cm,soil_moisture_9_to_27cm,soil_moisture_27_to_81cm&daily=weather_code,temperature_2m_max,temperature_2m_min,apparent_temperature_max,apparent_temperature_min,sunrise,sunset,daylight_duration,sunshine_duration,uv_index_max,uv_index_clear_sky_max,precipitation_sum,rain_sum,showers_sum,snowfall_sum,precipitation_hours,precipitation_probability_max,wind_speed_10m_max,wind_gusts_10m_max,wind_direction_10m_dominant,shortwave_radiation_sum,et0_fao_evapotranspiration&timeformat=unixtime&timezone=Europe%2FRome
                    //FormattableString addressUrlFormattable = $"https://api.open-meteo.com/v1/forecast?latitude={geo?.lat}&longitude={geo?.lon}&current=temperature_2m,relative_humidity_2m,apparent_temperature,is_day,precipitation,rain,showers,snowfall,weather_code,cloud_cover,pressure_msl,surface_pressure,wind_speed_10m,wind_direction_10m,wind_gusts_10m&hourly=temperature_2m,relative_humidity_2m,dew_point_2m,apparent_temperature,precipitation_probability,precipitation,rain,showers,snowfall,snow_depth,weather_code,pressure_msl,surface_pressure,cloud_cover,cloud_cover_low,cloud_cover_mid,cloud_cover_high,visibility,evapotranspiration,et0_fao_evapotranspiration,vapour_pressure_deficit,wind_speed_10m,wind_speed_80m,wind_speed_120m,wind_speed_180m,wind_direction_10m,wind_direction_80m,wind_direction_120m,wind_direction_180m,wind_gusts_10m,temperature_80m,temperature_120m,temperature_180m,soil_temperature_0cm,soil_temperature_6cm,soil_temperature_18cm,soil_temperature_54cm,soil_moisture_0_to_1cm,soil_moisture_1_to_3cm,soil_moisture_3_to_9cm,soil_moisture_9_to_27cm,soil_moisture_27_to_81cm&daily=weather_code,temperature_2m_max,temperature_2m_min,apparent_temperature_max,apparent_temperature_min,sunrise,sunset,daylight_duration,sunshine_duration,uv_index_max,uv_index_clear_sky_max,precipitation_sum,rain_sum,showers_sum,snowfall_sum,precipitation_hours,precipitation_probability_max,wind_speed_10m_max,wind_gusts_10m_max,wind_direction_10m_dominant,shortwave_radiation_sum,et0_fao_evapotranspiration&timeformat=unixtime&timezone=Europe%2FRome
                    //string addressUrl = FormattableString.Invariant(addressUrlFormattable);

                    //richiesta di quello che serve per questo esempio:
                    //https://api.open-meteo.com/v1/forecast?latitude={geo?.lat}&longitude={geo?.lon}&current=temperature_2m,weather_code,wind_speed_10m,wind_direction_10m&hourly=temperature_2m,relative_humidity_2m,dew_point_2m,apparent_temperature,precipitation_probability,precipitation,rain,showers,weather_code,wind_speed_10m,wind_direction_10m&daily=weather_code,temperature_2m_max,temperature_2m_min,apparent_temperature_max,apparent_temperature_min&timeformat=unixtime&timezone=auto";

                    //https://learn.microsoft.com/en-us/dotnet/standard/base-types/best-practices-display-data#display-formatted-data
                    //https://learn.microsoft.com/en-us/dotnet/api/system.formattablestring.invariant
                    //per evitare di avere problemi con la cultura in uso (ad esempio con l'italiano che usa la virgola come separatore decimale) creiamo prima una FormattableString e poi la convertiamo in una stringa con una cultura English Invariant 
                    FormattableString addressUrlFormattable = $"https://api.open-meteo.com/v1/forecast?latitude={geo?.lat}&longitude={geo?.lon}&current=temperature_2m,weather_code,wind_speed_10m,wind_direction_10m&hourly=temperature_2m,relative_humidity_2m,dew_point_2m,apparent_temperature,precipitation_probability,precipitation,rain,showers,weather_code,wind_speed_10m,wind_direction_10m&daily=weather_code,temperature_2m_max,temperature_2m_min,apparent_temperature_max,apparent_temperature_min&timeformat=unixtime&timezone=auto";
                    string addressUrl = FormattableString.Invariant(addressUrlFormattable);
                    //string addressUrl = addressUrlFormattable.ToString(CultureInfo.InvariantCulture);
                    //se volessi la cultura italiana (in questo caso non funzionerebbe la chiamata all'endpoint remoto)
                    //string addressUrl = addressUrlFormattable.ToString(new CultureInfo("it-IT"));
                    var response = await _client.GetAsync($"{addressUrl}");
                    if (response.IsSuccessStatusCode)
                    {
                        //https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-6-0#httpclient-and-httpcontent-extension-methods
                        //HttpCliet già usa le impostazioni web defaults come JsonSerializerOptions.
                        OpenMeteoForecast? forecast = await response.Content.ReadFromJsonAsync<OpenMeteoForecast>();
                        if (forecast != null)
                        {
                            ////per stampare a console usiamo i web defaults
                            //JsonSerializerOptions options = new(JsonSerializerDefaults.Web) { WriteIndented = true };
                            //Console.WriteLine("Dati ricevuti dall'endpoint remoto:\n" + JsonSerializer.Serialize(forecast, options));

                            Console.WriteLine($"\nCondizioni meteo attuali per {place}");
                            Console.WriteLine($"Latitutide: {forecast.Latitude}; Longitudine: {forecast.Longitude}; Elevazione: {forecast.Elevation} m; TimeZone: {forecast.Timezone}");
                            if (forecast.Current != null)
                            {
                                Console.WriteLine($"Data e ora previsione: {Utils.Display(Utils.UnixTimeStampToDateTime(forecast.Current.Time), datoNonFornitoString)}");
                                Console.WriteLine($"Temperatura : {Utils.Display(forecast.Current.Temperature2m, datoNonFornitoString)} °C");
                                Console.WriteLine($"previsione: {Utils.Display(Utils.WMOCodesIntIT(forecast.Current.WeatherCode), datoNonFornitoString)}");
                                Console.WriteLine($"Direzione del vento: {Utils.Display(forecast.Current.WindDirection10m, datoNonFornitoString)} °");
                                Console.WriteLine($"Velocità del vento: {Utils.Display(forecast.Current.WindSpeed10m, datoNonFornitoString)} Km/h");
                            }

                            if (forecast.Daily != null)
                            {
                                Console.WriteLine($"\nPrevisioni meteo giornaliere per {place}");
                                int? numeroGiorni = forecast.Daily.Time?.Count;
                                if (numeroGiorni > 0)
                                {
                                    for (int i = 0; i < numeroGiorni; i++)
                                    {
                                        Console.WriteLine($"Data e ora = {Utils.Display(Utils.UnixTimeStampToDateTime(forecast.Daily?.Time?[i]), datoNonFornitoString)};" +
                                            $" T max = {Utils.Display(forecast.Daily?.Temperature2mMax?[i], datoNonFornitoString)} °C;" +
                                            $" T min = {Utils.Display(forecast.Daily?.Temperature2mMin?[i], datoNonFornitoString)} °C; " +
                                            $"previsione = {Utils.Display(Utils.WMOCodesIntIT(forecast.Daily?.WeatherCode?[i]), datoNonFornitoString)}");
                                    }
                                }
                            }
                            if (forecast.Hourly != null)
                            {
                                Console.WriteLine($"\nPrevisioni meteo ora per ora per {place}");
                                int? numeroPrevisioni = forecast.Hourly.Time?.Count;
                                if (numeroPrevisioni > 0)
                                {
                                    for (int i = 0; i < numeroPrevisioni; i++)
                                    {
                                        Console.WriteLine($"Data e ora = {Utils.Display(Utils.UnixTimeStampToDateTime(forecast.Hourly.Time?[i]), datoNonFornitoString)};" +
                                            $" T = {Utils.Display(forecast.Hourly.Temperature2m?[i], datoNonFornitoString)} °C;" +
                                            $" U = {Utils.Display(forecast.Hourly.RelativeHumidity2m?[i], datoNonFornitoString)} %;" +
                                            $" T percepita = {Utils.Display(forecast.Hourly.ApparentTemperature?[i], datoNonFornitoString)};" +
                                            $" Prob. di rovesci = {Utils.Display(forecast.Hourly.PrecipitationProbability?[i] * 1.0, datoNonFornitoString)} %; " +
                                            $" previsione = {Utils.Display(Utils.WMOCodesIntIT(forecast.Hourly.WeatherCode?[i]), datoNonFornitoString)}");
                                        if((i+1) % 24 == 0)
                                        {
                                            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException || ex is ArgumentException)
                {
                    Console.WriteLine(ex.Message + "\nIl recupero dei dati dal server non è riuscito");
                }
            }
        }
    }
}