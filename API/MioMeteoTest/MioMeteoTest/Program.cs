using HttpProxyControl;
using System.Data;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
namespace MioMeteoTest
{
    internal class Program
    {
        static readonly HttpClient _client = HttpProxyHelper.CreateHttpClient(setProxy: true);
        static async Task Main(string[] args)
        {
            string place = "Besana in Brianza";
            DateTime dateTime = DateTime.Now.AddDays(4);
            string richiesta = "pioverà";

            await Meteo(place, dateTime, richiesta);
        }
        static async Task Meteo(string place, DateTime dateTime, string richiesta)
        {
            const string datoNonFornitoString = "Dato non fornito";
            TimeSpan timeDifference = dateTime - DateTime.Now;
            int daysDifference = (int)timeDifference.TotalDays + 1;
            if(daysDifference <= 6)
            {
                try
                {
                    (double? lat, double? lon)? geo = await GeocodeByOpenMeteo(_client, place);

                    if (geo != null)
                    {
                        FormattableString addressUrlFormattable = $"https://api.open-meteo.com/v1/forecast?latitude={geo?.lat}&longitude={geo?.lon}&current=temperature_2m,weather_code,wind_speed_10m,wind_direction_10m&hourly=temperature_2m,relative_humidity_2m,dew_point_2m,apparent_temperature,precipitation_probability,precipitation,rain,showers,weather_code,wind_speed_10m,wind_direction_10m&daily=weather_code,temperature_2m_max,temperature_2m_min,apparent_temperature_max,apparent_temperature_min&timeformat=unixtime&timezone=auto";
                        string addressUrl = FormattableString.Invariant(addressUrlFormattable);
                        var response = await _client.GetAsync($"{addressUrl}");
                        if (response.IsSuccessStatusCode)
                        {
                            OpenMeteoForecast? forecast = await response.Content.ReadFromJsonAsync<OpenMeteoForecast>();
                            if (forecast != null)
                            {
                                Console.WriteLine($"\nCondizioni meteo attuali per {place}");
                                Console.WriteLine($"Latitutide: {forecast.Latitude}; Longitudine: {forecast.Longitude}; Elevazione: {forecast.Elevation} m; TimeZone: {forecast.Timezone}");
                                if (forecast.Current != null)
                                {
                                    Console.WriteLine($"Data e ora previsione: {Display(UnixTimeStampToDateTime(forecast.Current.Time), datoNonFornitoString)}");
                                    Console.WriteLine($"Temperatura : {Display(forecast.Current.Temperature2m, datoNonFornitoString)} °C");
                                    Console.WriteLine($"previsione: {Display(WMOCodesIntIT(forecast.Current.WeatherCode), datoNonFornitoString)}");
                                    Console.WriteLine($"Direzione del vento: {Display(forecast.Current.WindDirection10m, datoNonFornitoString)} °");
                                    Console.WriteLine($"Velocità del vento: {Display(forecast.Current.WindSpeed10m, datoNonFornitoString)} Km/h");
                                }

                                if (forecast.Daily != null)
                                {
                                    Console.WriteLine($"\nPrevisioni meteo tra {daysDifference} giorni per {place}");
                                    int? numeroGiorni = forecast.Daily.Time?.Count;
                                    if (numeroGiorni > 0)
                                    {
                                        if (daysDifference != 0)
                                        {
                                            Console.WriteLine($"Data e ora = {Display(UnixTimeStampToDateTime(forecast.Daily?.Time?[daysDifference]), datoNonFornitoString)};" +
                                                $" T max = {Display(forecast.Daily?.Temperature2mMax?[daysDifference], datoNonFornitoString)} °C;" +
                                                $" T min = {Display(forecast.Daily?.Temperature2mMin?[daysDifference], datoNonFornitoString)} °C; " +
                                                $"previsione = {Display(WMOCodesIntIT(forecast.Daily?.WeatherCode?[daysDifference]), datoNonFornitoString)}");
                                        }
                                    }
                                }
                                if (richiesta == "tempo")
                                {
                                    Console.WriteLine($"Data e ora = {Display(UnixTimeStampToDateTime(forecast.Daily?.Time?[daysDifference]), datoNonFornitoString)};" +
                                        $" T max = {Display(forecast.Daily?.Temperature2mMax?[daysDifference], datoNonFornitoString)} °C;" +
                                        $" T min = {Display(forecast.Daily?.Temperature2mMin?[daysDifference], datoNonFornitoString)} °C; " +
                                        $"previsione = {Display(WMOCodesIntIT(forecast.Daily?.WeatherCode?[daysDifference]), datoNonFornitoString)}");
                                }
                                if (richiesta == "pioverà")
                                {
                                    if (forecast.Daily?.WeatherCode?[daysDifference] >= 51 && forecast.Daily?.WeatherCode?[daysDifference] <= 64 || forecast.Daily?.WeatherCode?[daysDifference] >= 80 && forecast.Daily?.WeatherCode?[daysDifference] <= 99)
                                    {
                                        Console.WriteLine($"Previsioni per il {Display(UnixTimeStampToDateTime(forecast.Daily?.Time?[daysDifference]), datoNonFornitoString)} {Display(WMOCodesIntIT(forecast.Daily?.WeatherCode?[daysDifference]), datoNonFornitoString)}");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Non sono previste precipitazioni per i prossimi {daysDifference} giorni");
                                    }
                                }
                                if (richiesta == "nevicherà")
                                {
                                    if (forecast.Daily?.WeatherCode?[daysDifference] >= 71 && forecast.Daily?.WeatherCode?[daysDifference] <= 77)
                                    {
                                        Console.WriteLine($"Previsioni per il {Display(UnixTimeStampToDateTime(forecast.Daily?.Time?[daysDifference]), datoNonFornitoString)} {Display(WMOCodesIntIT(forecast.Daily?.WeatherCode?[daysDifference]), datoNonFornitoString)}");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Non sono previste nevicate per i prossimi {daysDifference} giorni");
                                    }
                                }
                                if (richiesta == "nebbia")
                                {
                                    if (forecast.Daily?.WeatherCode?[daysDifference] >= 45 && forecast.Daily?.WeatherCode?[daysDifference] <= 48)
                                    {
                                        Console.WriteLine($"Previsioni per il {Display(UnixTimeStampToDateTime(forecast.Daily?.Time?[daysDifference]), datoNonFornitoString)} {Display(WMOCodesIntIT(forecast.Daily?.WeatherCode?[daysDifference]), datoNonFornitoString)}");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Non è prevista nebbia per i prossimi {daysDifference} giorni");
                                    }
                                    if (richiesta == "nuvoloso" || richiesta == "sereno")
                                    {
                                        if (forecast.Daily?.WeatherCode?[daysDifference] >= 0 && forecast.Daily?.WeatherCode?[daysDifference] <= 3)
                                        {
                                            Console.WriteLine($"Previsioni per il {Display(UnixTimeStampToDateTime(forecast.Daily?.Time?[daysDifference]), datoNonFornitoString)} {Display(WMOCodesIntIT(forecast.Daily?.WeatherCode?[daysDifference]), datoNonFornitoString)}");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Il cielo non sarà {richiesta} per i prossimi {daysDifference} giorni");
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
            else
            {
                Console.WriteLine($"Hai chiesto le previsioni per il {dateTime} Dati non disponibili");
            }
        }
        #region Funzionamento
        #region util

        public static string Display(object? value, string? stringIfNull)
        {
            if (value is null)
            {
                if (stringIfNull is null)
                {
                    return string.Empty;
                }
                return stringIfNull;
            }
            else
            {
                if (stringIfNull is null)
                {
                    return value.ToString() ?? string.Empty;
                }
                return value.ToString() ?? stringIfNull;
            }
        }
        public static DateTime? UnixTimeStampToDateTime(double? unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            if (unixTimeStamp != null)
            {
                DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds((double)unixTimeStamp).ToLocalTime();
                return dateTime;
            }
            return null;
        }
        public static async Task<(double? lat, double? lon)?> GeocodeByOpenMeteo(HttpClient client, string? name, string? language = "it", int count = 1)
        {
            string? nameEncoded = HttpUtility.UrlEncode(name);
            string geocodingUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={nameEncoded}&count={count}&language={language}";
            try
            {
                HttpResponseMessage responseGeocoding = await client.GetAsync($"{geocodingUrl}");
                if (responseGeocoding.IsSuccessStatusCode)
                {
                    OpenMeteoGeocoding? geocodingResult = await responseGeocoding.Content.ReadFromJsonAsync<OpenMeteoGeocoding>();
                    if (geocodingResult != null && geocodingResult.Results?.Count > 0)
                    {
                        return (geocodingResult.Results[0].Latitude, geocodingResult.Results[0].Longitude);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException || ex is ArgumentException)
                {
                    Debug.WriteLine(ex.Message + "\nIl recupero dei dati dal server non è riuscito");
                }
            }
            return null;
        }
        public static string? WMOCodesIntIT(int? code)
        {
            string? result = code switch
            {
                0 => "cielo sereno",
                1 => "prevalentemente limpido",
                2 => "parzialmente nuvoloso",
                3 => "coperto",
                45 => "nebbia",
                48 => "nebbia con brina",
                51 => "pioggerellina di scarsa intensità",
                53 => "pioggerellina di moderata intensità",
                55 => "pioggerellina intensa",
                56 => "pioggerellina gelata di scarsa intensità",
                57 => "pioggerellina gelata intensa",
                61 => "pioggia di scarsa intensità",
                63 => "pioggia di moderata intensità",
                65 => "pioggia molto intensa",
                66 => "pioggia gelata di scarsa intensità",
                67 => "pioggia gelata intensa",
                71 => "nevicata di lieve entità",
                73 => "nevicata di media entità",
                75 => "nevicata intensa",
                77 => "granelli di neve",
                80 => "deboli rovesci di pioggia",
                81 => "moderati rovesci di pioggia",
                82 => "violenti rovesci di pioggia",
                85 => "leggeri rovesci di neve",
                86 => "pesanti rovesci di neve",
                95 => "temporale lieve o moderato",
                96 => "temporale con lieve grandine",
                99 => "temporale con forte grandine",
                _ => null,
            };
            return result;
        }
        #endregion
        #region geocoding

        public class Result
        {
            [JsonPropertyName("id")]
            public int? Id { get; set; }

            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("latitude")]
            public double? Latitude { get; set; }

            [JsonPropertyName("longitude")]
            public double? Longitude { get; set; }

            [JsonPropertyName("elevation")]
            public double? Elevation { get; set; }

            [JsonPropertyName("feature_code")]
            public string? FeatureCode { get; set; }

            [JsonPropertyName("country_code")]
            public string? CountryCode { get; set; }

            [JsonPropertyName("admin1_id")]
            public int? Admin1Id { get; set; }

            [JsonPropertyName("admin3_id")]
            public int? Admin3Id { get; set; }

            [JsonPropertyName("admin4_id")]
            public int? Admin4Id { get; set; }

            [JsonPropertyName("timezone")]
            public string? Timezone { get; set; }

            [JsonPropertyName("population")]
            public int? Population { get; set; }

            [JsonPropertyName("postcodes")]
            public List<string>? Postcodes { get; set; }

            [JsonPropertyName("country_id")]
            public int? CountryId { get; set; }

            [JsonPropertyName("country")]
            public string? Country { get; set; }

            [JsonPropertyName("admin1")]
            public string? Admin1 { get; set; }

            [JsonPropertyName("admin3")]
            public string? Admin3 { get; set; }

            [JsonPropertyName("admin4")]
            public string? Admin4 { get; set; }

            [JsonPropertyName("admin2_id")]
            public int? Admin2Id { get; set; }

            [JsonPropertyName("admin2")]
            public string? Admin2 { get; set; }
        }
        public class OpenMeteoGeocoding
        {
            [JsonPropertyName("results")]
            public List<Result>? Results { get; set; }

            [JsonPropertyName("generationtime_ms")]
            public double? GenerationtimeMs { get; set; }
        }
        #endregion
        #region completo

        public class Current
        {
            [JsonPropertyName("time")]
            public int? Time { get; set; }

            [JsonPropertyName("interval")]
            public int? Interval { get; set; }

            [JsonPropertyName("temperature_2m")]
            public double? Temperature2m { get; set; }

            [JsonPropertyName("relative_humidity_2m")]
            public int? RelativeHumidity2m { get; set; }

            [JsonPropertyName("apparent_temperature")]
            public double? ApparentTemperature { get; set; }

            [JsonPropertyName("is_day")]
            public int? IsDay { get; set; }

            [JsonPropertyName("precipitation")]
            public int? Precipitation { get; set; }

            [JsonPropertyName("rain")]
            public int? Rain { get; set; }

            [JsonPropertyName("showers")]
            public int? Showers { get; set; }

            [JsonPropertyName("snowfall")]
            public int? Snowfall { get; set; }

            [JsonPropertyName("weather_code")]
            public int? WeatherCode { get; set; }

            [JsonPropertyName("cloud_cover")]
            public int? CloudCover { get; set; }

            [JsonPropertyName("pressure_msl")]
            public double? PressureMsl { get; set; }

            [JsonPropertyName("surface_pressure")]
            public double? SurfacePressure { get; set; }

            [JsonPropertyName("wind_speed_10m")]
            public double? WindSpeed10m { get; set; }

            [JsonPropertyName("wind_direction_10m")]
            public int? WindDirection10m { get; set; }

            [JsonPropertyName("wind_gusts_10m")]
            public double? WindGusts10m { get; set; }
        }

        public class CurrentUnits
        {
            [JsonPropertyName("time")]
            public string? Time { get; set; }

            [JsonPropertyName("interval")]
            public string? Interval { get; set; }

            [JsonPropertyName("temperature_2m")]
            public string? Temperature2m { get; set; }

            [JsonPropertyName("relative_humidity_2m")]
            public string? RelativeHumidity2m { get; set; }

            [JsonPropertyName("apparent_temperature")]
            public string? ApparentTemperature { get; set; }

            [JsonPropertyName("is_day")]
            public string? IsDay { get; set; }

            [JsonPropertyName("precipitation")]
            public string? Precipitation { get; set; }

            [JsonPropertyName("rain")]
            public string? Rain { get; set; }

            [JsonPropertyName("showers")]
            public string? Showers { get; set; }

            [JsonPropertyName("snowfall")]
            public string? Snowfall { get; set; }

            [JsonPropertyName("weather_code")]
            public string? WeatherCode { get; set; }

            [JsonPropertyName("cloud_cover")]
            public string? CloudCover { get; set; }

            [JsonPropertyName("pressure_msl")]
            public string? PressureMsl { get; set; }

            [JsonPropertyName("surface_pressure")]
            public string? SurfacePressure { get; set; }

            [JsonPropertyName("wind_speed_10m")]
            public string? WindSpeed10m { get; set; }

            [JsonPropertyName("wind_direction_10m")]
            public string? WindDirection10m { get; set; }

            [JsonPropertyName("wind_gusts_10m")]
            public string? WindGusts10m { get; set; }
        }

        public class Daily
        {
            [JsonPropertyName("time")]
            public List<int>? Time { get; set; }

            [JsonPropertyName("weather_code")]
            public List<int>? WeatherCode { get; set; }

            [JsonPropertyName("temperature_2m_max")]
            public List<double>? Temperature2mMax { get; set; }

            [JsonPropertyName("temperature_2m_min")]
            public List<double>? Temperature2mMin { get; set; }

            [JsonPropertyName("apparent_temperature_max")]
            public List<double>? ApparentTemperatureMax { get; set; }

            [JsonPropertyName("apparent_temperature_min")]
            public List<double>? ApparentTemperatureMin { get; set; }

            [JsonPropertyName("sunrise")]
            public List<int>? Sunrise { get; set; }

            [JsonPropertyName("sunset")]
            public List<int>? Sunset { get; set; }

            [JsonPropertyName("daylight_duration")]
            public List<double>? DaylightDuration { get; set; }

            [JsonPropertyName("sunshine_duration")]
            public List<double>? SunshineDuration { get; set; }

            [JsonPropertyName("uv_index_max")]
            public List<double>? UvIndexMax { get; set; }

            [JsonPropertyName("uv_index_clear_sky_max")]
            public List<double>? UvIndexClearSkyMax { get; set; }

            [JsonPropertyName("precipitation_sum")]
            public List<double>? PrecipitationSum { get; set; }

            [JsonPropertyName("rain_sum")]
            public List<double>? RainSum { get; set; }

            [JsonPropertyName("showers_sum")]
            public List<double>? ShowersSum { get; set; }

            [JsonPropertyName("snowfall_sum")]
            public List<int>? SnowfallSum { get; set; }

            [JsonPropertyName("precipitation_hours")]
            public List<int>? PrecipitationHours { get; set; }

            [JsonPropertyName("precipitation_probability_max")]
            public List<int>? PrecipitationProbabilityMax { get; set; }

            [JsonPropertyName("wind_speed_10m_max")]
            public List<double>? WindSpeed10mMax { get; set; }

            [JsonPropertyName("wind_gusts_10m_max")]
            public List<double>? WindGusts10mMax { get; set; }

            [JsonPropertyName("wind_direction_10m_dominant")]
            public List<int>? WindDirection10mDominant { get; set; }

            [JsonPropertyName("shortwave_radiation_sum")]
            public List<double>? ShortwaveRadiationSum { get; set; }

            [JsonPropertyName("et0_fao_evapotranspiration")]
            public List<double>? Et0FaoEvapotranspiration { get; set; }
        }

        public class DailyUnits
        {
            [JsonPropertyName("time")]
            public string? Time { get; set; }

            [JsonPropertyName("weather_code")]
            public string? WeatherCode { get; set; }

            [JsonPropertyName("temperature_2m_max")]
            public string? Temperature2mMax { get; set; }

            [JsonPropertyName("temperature_2m_min")]
            public string? Temperature2mMin { get; set; }

            [JsonPropertyName("apparent_temperature_max")]
            public string? ApparentTemperatureMax { get; set; }

            [JsonPropertyName("apparent_temperature_min")]
            public string? ApparentTemperatureMin { get; set; }

            [JsonPropertyName("sunrise")]
            public string? Sunrise { get; set; }

            [JsonPropertyName("sunset")]
            public string? Sunset { get; set; }

            [JsonPropertyName("daylight_duration")]
            public string? DaylightDuration { get; set; }

            [JsonPropertyName("sunshine_duration")]
            public string? SunshineDuration { get; set; }

            [JsonPropertyName("uv_index_max")]
            public string? UvIndexMax { get; set; }

            [JsonPropertyName("uv_index_clear_sky_max")]
            public string? UvIndexClearSkyMax { get; set; }

            [JsonPropertyName("precipitation_sum")]
            public string? PrecipitationSum { get; set; }

            [JsonPropertyName("rain_sum")]
            public string? RainSum { get; set; }

            [JsonPropertyName("showers_sum")]
            public string? ShowersSum { get; set; }

            [JsonPropertyName("snowfall_sum")]
            public string? SnowfallSum { get; set; }

            [JsonPropertyName("precipitation_hours")]
            public string? PrecipitationHours { get; set; }

            [JsonPropertyName("precipitation_probability_max")]
            public string? PrecipitationProbabilityMax { get; set; }

            [JsonPropertyName("wind_speed_10m_max")]
            public string? WindSpeed10mMax { get; set; }

            [JsonPropertyName("wind_gusts_10m_max")]
            public string? WindGusts10mMax { get; set; }

            [JsonPropertyName("wind_direction_10m_dominant")]
            public string? WindDirection10mDominant { get; set; }

            [JsonPropertyName("shortwave_radiation_sum")]
            public string? ShortwaveRadiationSum { get; set; }

            [JsonPropertyName("et0_fao_evapotranspiration")]
            public string? Et0FaoEvapotranspiration { get; set; }
        }

        public class Hourly
        {
            [JsonPropertyName("time")]
            public List<int>? Time { get; set; }

            [JsonPropertyName("temperature_2m")]
            public List<double>? Temperature2m { get; set; }

            [JsonPropertyName("relative_humidity_2m")]
            public List<int>? RelativeHumidity2m { get; set; }

            [JsonPropertyName("dew_point_2m")]
            public List<double>? DewPoint2m { get; set; }

            [JsonPropertyName("apparent_temperature")]
            public List<double>? ApparentTemperature { get; set; }

            [JsonPropertyName("precipitation_probability")]
            public List<int>? PrecipitationProbability { get; set; }

            [JsonPropertyName("precipitation")]
            public List<double>? Precipitation { get; set; }

            [JsonPropertyName("rain")]
            public List<double>? Rain { get; set; }

            [JsonPropertyName("showers")]
            public List<double>? Showers { get; set; }

            [JsonPropertyName("snowfall")]
            public List<int>? Snowfall { get; set; }

            [JsonPropertyName("snow_depth")]
            public List<int>? SnowDepth { get; set; }

            [JsonPropertyName("weather_code")]
            public List<int>? WeatherCode { get; set; }

            [JsonPropertyName("pressure_msl")]
            public List<double>? PressureMsl { get; set; }

            [JsonPropertyName("surface_pressure")]
            public List<double>? SurfacePressure { get; set; }

            [JsonPropertyName("cloud_cover")]
            public List<int>? CloudCover { get; set; }

            [JsonPropertyName("cloud_cover_low")]
            public List<int>? CloudCoverLow { get; set; }

            [JsonPropertyName("cloud_cover_mid")]
            public List<int>? CloudCoverMid { get; set; }

            [JsonPropertyName("cloud_cover_high")]
            public List<int>? CloudCoverHigh { get; set; }

            [JsonPropertyName("visibility")]
            public List<int>? Visibility { get; set; }

            [JsonPropertyName("evapotranspiration")]
            public List<double>? Evapotranspiration { get; set; }

            [JsonPropertyName("et0_fao_evapotranspiration")]
            public List<double>? Et0FaoEvapotranspiration { get; set; }

            [JsonPropertyName("vapour_pressure_deficit")]
            public List<double>? VapourPressureDeficit { get; set; }

            [JsonPropertyName("wind_speed_10m")]
            public List<double>? WindSpeed10m { get; set; }

            [JsonPropertyName("wind_speed_80m")]
            public List<double>? WindSpeed80m { get; set; }

            [JsonPropertyName("wind_speed_120m")]
            public List<double>? WindSpeed120m { get; set; }

            [JsonPropertyName("wind_speed_180m")]
            public List<double>? WindSpeed180m { get; set; }

            [JsonPropertyName("wind_direction_10m")]
            public List<int>? WindDirection10m { get; set; }

            [JsonPropertyName("wind_direction_80m")]
            public List<int>? WindDirection80m { get; set; }

            [JsonPropertyName("wind_direction_120m")]
            public List<int>? WindDirection120m { get; set; }

            [JsonPropertyName("wind_direction_180m")]
            public List<int>? WindDirection180m { get; set; }

            [JsonPropertyName("wind_gusts_10m")]
            public List<double>? WindGusts10m { get; set; }

            [JsonPropertyName("temperature_80m")]
            public List<double>? Temperature80m { get; set; }

            [JsonPropertyName("temperature_120m")]
            public List<double>? Temperature120m { get; set; }

            [JsonPropertyName("temperature_180m")]
            public List<double>? Temperature180m { get; set; }

            [JsonPropertyName("soil_temperature_0cm")]
            public List<double>? SoilTemperature0cm { get; set; }

            [JsonPropertyName("soil_temperature_6cm")]
            public List<double>? SoilTemperature6cm { get; set; }

            [JsonPropertyName("soil_temperature_18cm")]
            public List<double>? SoilTemperature18cm { get; set; }

            [JsonPropertyName("soil_temperature_54cm")]
            public List<double>? SoilTemperature54cm { get; set; }

            [JsonPropertyName("soil_moisture_0_to_1cm")]
            public List<double>? SoilMoisture0To1cm { get; set; }

            [JsonPropertyName("soil_moisture_1_to_3cm")]
            public List<double>? SoilMoisture1To3cm { get; set; }

            [JsonPropertyName("soil_moisture_3_to_9cm")]
            public List<double>? SoilMoisture3To9cm { get; set; }

            [JsonPropertyName("soil_moisture_9_to_27cm")]
            public List<double>? SoilMoisture9To27cm { get; set; }

            [JsonPropertyName("soil_moisture_27_to_81cm")]
            public List<double>? SoilMoisture27To81cm { get; set; }
        }

        public class HourlyUnits
        {
            [JsonPropertyName("time")]
            public string? Time { get; set; }

            [JsonPropertyName("temperature_2m")]
            public string? Temperature2m { get; set; }

            [JsonPropertyName("relative_humidity_2m")]
            public string? RelativeHumidity2m { get; set; }

            [JsonPropertyName("dew_point_2m")]
            public string? DewPoint2m { get; set; }

            [JsonPropertyName("apparent_temperature")]
            public string? ApparentTemperature { get; set; }

            [JsonPropertyName("precipitation_probability")]
            public string? PrecipitationProbability { get; set; }

            [JsonPropertyName("precipitation")]
            public string? Precipitation { get; set; }

            [JsonPropertyName("rain")]
            public string? Rain { get; set; }

            [JsonPropertyName("showers")]
            public string? Showers { get; set; }

            [JsonPropertyName("snowfall")]
            public string? Snowfall { get; set; }

            [JsonPropertyName("snow_depth")]
            public string? SnowDepth { get; set; }

            [JsonPropertyName("weather_code")]
            public string? WeatherCode { get; set; }

            [JsonPropertyName("pressure_msl")]
            public string? PressureMsl { get; set; }

            [JsonPropertyName("surface_pressure")]
            public string? SurfacePressure { get; set; }

            [JsonPropertyName("cloud_cover")]
            public string? CloudCover { get; set; }

            [JsonPropertyName("cloud_cover_low")]
            public string? CloudCoverLow { get; set; }

            [JsonPropertyName("cloud_cover_mid")]
            public string? CloudCoverMid { get; set; }

            [JsonPropertyName("cloud_cover_high")]
            public string? CloudCoverHigh { get; set; }

            [JsonPropertyName("visibility")]
            public string? Visibility { get; set; }

            [JsonPropertyName("evapotranspiration")]
            public string? Evapotranspiration { get; set; }

            [JsonPropertyName("et0_fao_evapotranspiration")]
            public string? Et0FaoEvapotranspiration { get; set; }

            [JsonPropertyName("vapour_pressure_deficit")]
            public string? VapourPressureDeficit { get; set; }

            [JsonPropertyName("wind_speed_10m")]
            public string? WindSpeed10m { get; set; }

            [JsonPropertyName("wind_speed_80m")]
            public string? WindSpeed80m { get; set; }

            [JsonPropertyName("wind_speed_120m")]
            public string? WindSpeed120m { get; set; }

            [JsonPropertyName("wind_speed_180m")]
            public string? WindSpeed180m { get; set; }

            [JsonPropertyName("wind_direction_10m")]
            public string? WindDirection10m { get; set; }

            [JsonPropertyName("wind_direction_80m")]
            public string? WindDirection80m { get; set; }

            [JsonPropertyName("wind_direction_120m")]
            public string? WindDirection120m { get; set; }

            [JsonPropertyName("wind_direction_180m")]
            public string? WindDirection180m { get; set; }

            [JsonPropertyName("wind_gusts_10m")]
            public string? WindGusts10m { get; set; }

            [JsonPropertyName("temperature_80m")]
            public string? Temperature80m { get; set; }

            [JsonPropertyName("temperature_120m")]
            public string? Temperature120m { get; set; }

            [JsonPropertyName("temperature_180m")]
            public string? Temperature180m { get; set; }

            [JsonPropertyName("soil_temperature_0cm")]
            public string? SoilTemperature0cm { get; set; }

            [JsonPropertyName("soil_temperature_6cm")]
            public string? SoilTemperature6cm { get; set; }

            [JsonPropertyName("soil_temperature_18cm")]
            public string? SoilTemperature18cm { get; set; }

            [JsonPropertyName("soil_temperature_54cm")]
            public string? SoilTemperature54cm { get; set; }

            [JsonPropertyName("soil_moisture_0_to_1cm")]
            public string? SoilMoisture0To1cm { get; set; }

            [JsonPropertyName("soil_moisture_1_to_3cm")]
            public string? SoilMoisture1To3cm { get; set; }

            [JsonPropertyName("soil_moisture_3_to_9cm")]
            public string? SoilMoisture3To9cm { get; set; }

            [JsonPropertyName("soil_moisture_9_to_27cm")]
            public string? SoilMoisture9To27cm { get; set; }

            [JsonPropertyName("soil_moisture_27_to_81cm")]
            public string? SoilMoisture27To81cm { get; set; }
        }

        public class OpenMeteoForecast
        {
            [JsonPropertyName("latitude")]
            public double? Latitude { get; set; }

            [JsonPropertyName("longitude")]
            public double? Longitude { get; set; }

            [JsonPropertyName("generationtime_ms")]
            public double? GenerationtimeMs { get; set; }

            [JsonPropertyName("utc_offset_seconds")]
            public int? UtcOffsetSeconds { get; set; }

            [JsonPropertyName("timezone")]
            public string? Timezone { get; set; }

            [JsonPropertyName("timezone_abbreviation")]
            public string? TimezoneAbbreviation { get; set; }

            [JsonPropertyName("elevation")]
            public double? Elevation { get; set; }

            [JsonPropertyName("current_units")]
            public CurrentUnits? CurrentUnits { get; set; }

            [JsonPropertyName("current")]
            public Current? Current { get; set; }

            [JsonPropertyName("hourly_units")]
            public HourlyUnits? HourlyUnits { get; set; }

            [JsonPropertyName("hourly")]
            public Hourly? Hourly { get; set; }

            [JsonPropertyName("daily_units")]
            public DailyUnits? DailyUnits { get; set; }

            [JsonPropertyName("daily")]
            public Daily? Daily { get; set; }
        }
        #endregion
        #endregion
    }
}
