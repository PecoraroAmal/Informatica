using System.Text.Json;

namespace JSON
{
    internal class Program
    {
        public class Operator
        {
            public string Name { get; set; }
            public string Star { get; set; }
        }
        static async Task Main(string[] args)
        {
            await Console.Out.WriteLineAsync("Serializzazione");
            Serializzazione();
            await Console.Out.WriteLineAsync("DeSerializzazione");
            DeSerializzazione();
        }
        public static void Serializzazione()
        {
            List<Operator> operi = new List<Operator>();
            var operi1 = new Operator
            {
                Name = "Mostima",
                Star = "6"
            };
            var operi2 = new Operator
            {
                Name = "Exusiai",
                Star = "6"
            };
            var operi3 = new Operator
            {
                Name = "LaPlume",
                Star = "5"
            };
            operi.Add(operi1);
            operi.Add(operi2);
            operi.Add(operi3);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string serializzazione = JsonSerializer.Serialize(operi,options);
            File.WriteAllText("Ark.json", serializzazione);
            Console.WriteLine(serializzazione);
        }
        public static void DeSerializzazione()
        {
            string text = File.ReadAllText("Ark.json");
            Console.WriteLine(text);
            List<Operator>? operi = JsonSerializer.Deserialize<List<Operator>>(text);
            foreach(var op in operi)
            {
                Console.WriteLine($"Nome: {op.Name}");
                Console.WriteLine($"Rarità: {op.Star}");
            }
        }
    }
}
