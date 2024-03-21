using System.Diagnostics;
using System.Text.Json;
//esempio di deserializzazione da oggetti JSON a oggetti .NET
//https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-7-0#how-to-read-json-as-net-objects-deserialize

namespace JSONDemo2
{
    public class WeatherForecast
    {
        public DateTimeOffset Date { get; set; }
        public int TemperatureCelsius { get; set; }
        public string? Summary { get; set; }
        public string? SummaryField;
        public IList<DateTimeOffset>? DatesAvailable { get; set; }
        public Dictionary<string, HighLowTemps>? TemperatureRanges { get; set; }
        public string[]? SummaryWords { get; set; }
    }

    public class HighLowTemps
    {
        public int High { get; set; }
        public int Low { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("----------------------Es1----------------------");
            DeserializzazioneJSONEsempio1();
            Console.WriteLine("----------------------Es2----------------------");
            DeserializzazioneJSONDaFileEsempio1();
            Console.WriteLine("----------------------Es3----------------------");
            await DeserializzazioneJSONDaFileAsyncEsempio1();

        }

        private static async Task DeserializzazioneJSONDaFileAsyncEsempio1()
        {
            //il file WeatherForecasts.json deve esistere e deve contenere un array di oggetti JSON di tipo WeatherForecasts
            try
            {
                string fileName = "WeatherForecasts.json";
                using FileStream fileStream = File.OpenRead(fileName);
                List<WeatherForecast>? weatherForecasts =
                    await JsonSerializer.DeserializeAsync<List<WeatherForecast>>(fileStream);
                //List<WeatherForecast>? weatherForecasts = JsonSerializer.Deserialize<List<WeatherForecast>>(jsonString);
                if (weatherForecasts != null)
                {
                    foreach (var weatherForecast in weatherForecasts)
                    {
                        Console.WriteLine($"Date: {weatherForecast.Date}");
                        Console.WriteLine($"TemperatureCelsius: {weatherForecast.TemperatureCelsius}");
                        Console.WriteLine($"Summary: {weatherForecast.Summary}");
                        Console.WriteLine("-----");
                    }
                }

            }
            catch (JsonException e)
            {

                Debug.WriteLine(e.Message);
            }
        }

        private static void DeserializzazioneJSONDaFileEsempio1()
        {
            //il file WeatherForecasts.json deve esistere e deve contenere un array di oggetti JSON di tipo WeatherForecasts
            try
            {
                string fileName = "WeatherForecasts.json";
                string jsonString = File.ReadAllText(fileName);
                List<WeatherForecast>? weatherForecasts = JsonSerializer.Deserialize<List<WeatherForecast>>(jsonString);
                if (weatherForecasts != null)
                {
                    foreach (var weatherForecast in weatherForecasts)
                    {
                        Console.WriteLine($"Date: {weatherForecast.Date}");
                        Console.WriteLine($"TemperatureCelsius: {weatherForecast.TemperatureCelsius}");
                        Console.WriteLine($"Summary: {weatherForecast.Summary}");
                        Console.WriteLine("-----");
                    }
                }

            }
            catch (JsonException e)
            {

                Debug.WriteLine(e.Message);
            }
        }

        private static void DeserializzazioneJSONEsempio1()
        {
            string jsonString =
                        @"{
              ""Date"": ""2019-08-01T00:00:00-07:00"",
              ""TemperatureCelsius"": 25,
              ""Summary"": ""Hot"",
              ""DatesAvailable"": [
                ""2019-08-01T00:00:00-07:00"",
                ""2019-08-02T00:00:00-07:00""
              ],
              ""TemperatureRanges"": {
                            ""Cold"": {
                                ""High"": 20,
                  ""Low"": -10
                            },
                ""Hot"": {
                                ""High"": 60,
                  ""Low"": 20
                }
                        },
              ""SummaryWords"": [
                ""Cool"",
                ""Windy"",
                ""Humid""
              ]
            }
            ";

            WeatherForecast? weatherForecast =
                JsonSerializer.Deserialize<WeatherForecast>(jsonString);

            Console.WriteLine($"Date: {weatherForecast?.Date}");
            Console.WriteLine($"TemperatureCelsius: {weatherForecast?.TemperatureCelsius}");
            Console.WriteLine($"Summary: {weatherForecast?.Summary}");
        }
    }
}
