using System.Text.Json;
//esempio di serializzazione da oggetti .NET a oggetti JSON
//https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to?pivots=dotnet-7-0

namespace JSONDemo1
{
    //il modello dei dati - POCO solo proprietà, niente metodi, ereditarietà
    public class WeatherForecast
    {
        public DateTimeOffset Date { get; set; }
        public int TemperatureCelsius { get; set; }
        public string? Summary { get; set; }
    }
    class Program
    {

        static async Task Main(string[] args)
        {
            //versione sincrona
            JSONDemoEsempio1();
            await Console.Out.WriteLineAsync("-------------------------------------------");
            //Versione asincrona
            await JSONDemoAsyncEsempio1();
        }

        private static void JSONDemoEsempio1()
        {
            var weatherForecast = new WeatherForecast
            {
                Date = DateTime.Parse("2019-08-01"),
                TemperatureCelsius = 25,
                Summary = "Hot"
            };
            var weatherForecast2 = new WeatherForecast
            {
                Date = DateTime.Parse("2019-08-02"),
                TemperatureCelsius = 30,
                Summary = "Very Hot"
            };
            List<WeatherForecast> previsioni = new() { weatherForecast, weatherForecast2 };

            //serializzazione: da oggetto .NET a oggetto JSON
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(weatherForecast, options);
            Console.WriteLine(jsonString);
            //serializzazione di una collection in JSON
            Console.WriteLine("\n");
            string jsonCollectionString = JsonSerializer.Serialize(previsioni, options);
            Console.WriteLine(jsonCollectionString);
            //salviamo su file la collection
            string fileName = "WeatherForecasts.json";
            File.WriteAllText(fileName, jsonCollectionString);
            //leggo dal file e stampo a console
            Console.WriteLine("Lettura del JSON da file");
            Console.WriteLine(File.ReadAllText(fileName));
        }

        private static async Task JSONDemoAsyncEsempio1()
        {
            var weatherForecast = new WeatherForecast
            {
                Date = DateTime.Parse("2019-08-01"),
                TemperatureCelsius = 25,
                Summary = "Hot"
            };
            var weatherForecast2 = new WeatherForecast
            {
                Date = DateTime.Parse("2019-08-02"),
                TemperatureCelsius = 30,
                Summary = "Very Hot"
            };
            List<WeatherForecast> previsioni = new() { weatherForecast, weatherForecast2 };

            //serializzazione: da oggetto .NET a oggetto JSON
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(weatherForecast, options);
            Console.WriteLine(jsonString);
            //serializzazione di una collection in JSON
            Console.WriteLine("\n");
            string jsonCollectionString = JsonSerializer.Serialize(previsioni, options);
            Console.WriteLine(jsonCollectionString);
            //salviamo su file la collection
            string fileName = "WeatherForecasts.json";
            FileStream fileStream = File.Create(fileName);
            //Impostiamo il flusso di serializzazione JSON direttamente sullo stream che punta al file
            await JsonSerializer.SerializeAsync(fileStream, previsioni, options);
            await fileStream.DisposeAsync();
            //File.WriteAllText(fileName, jsonCollectionString);
            //leggo dal file e stampo a console
            Console.WriteLine("Lettura del JSON da file");

            Console.WriteLine(await File.ReadAllTextAsync(fileName));

        }

    }
}