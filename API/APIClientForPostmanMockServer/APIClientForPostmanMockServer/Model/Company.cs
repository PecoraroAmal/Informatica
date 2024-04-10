using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APIClientForPostmanMockServer.Model
{
    public class Company
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("revenue")]
        public double Revenue { get; set; }

        [JsonPropertyName("headquarter")]
        public Headquarter? Headquarter { get; set; }

        [JsonPropertyName("locations")]
        public List<Location>? Locations { get; set; }
    }
    public class Headquarter
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("employee_number")]
        public int EmployeeNumber { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }
    }
}
