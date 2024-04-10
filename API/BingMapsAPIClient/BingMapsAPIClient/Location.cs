using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BingMapsAPIClient
{
    public class Location
    {
        public string AuthenticationResultCode { get; set; } = null!;
        public string BrandLogoUri { get; set; } = null!;
        public string Copyright { get; set; } = null!;
        public ResourceSet[]? ResourceSets { get; set; }
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; } = null!;
        public string TraceId { get; set; } = null!;
    }

    public class ResourceSet
    {
        public int EstimatedTotal { get; set; }
        public Resource[]? Resources { get; set; }
    }

    public class Resource
    {
        [JsonPropertyName("__type")]
        public string? Type { get; set; }
        public float[]? Bbox { get; set; }
        public string? Name { get; set; }
        public Point? Point { get; set; }
        public Address? Address { get; set; }
        public string? Confidence { get; set; }
        public string? EntityType { get; set; }
        public GeocodePoint[]? GeocodePoints { get; set; }
        public string[]? MatchCodes { get; set; }
    }

    public class Point
    {
        public string Type { get; set; } = null!;
        public double[] Coordinates { get; set; } = null!;
    }

    public class Address
    {
        public string? AddressLine { get; set; }
        public string? AdminDistrict { get; set; }
        public string? AdminDistrict2 { get; set; }
        public string? CountryRegion { get; set; }
        public string? FormattedAddress { get; set; }
        public string? Locality { get; set; }
        public string? PostalCode { get; set; }
        public string? CountryRegionIso2 { get; set; }
    }

    public class GeocodePoint
    {
        public string Type { get; set; } = null!;
        public double[] Coordinates { get; set; } = null!;
        public string CalculationMethod { get; set; } = null!;
        public string[] UsageTypes { get; set; } = null!;
    }

}
