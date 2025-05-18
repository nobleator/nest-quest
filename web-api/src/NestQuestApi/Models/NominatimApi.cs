using System.Text.Json.Serialization;

namespace NestQuestApi.Models;

public class NominatimApiResponse
{
    [JsonPropertyName("place_id")]
    public long PlaceId { get; set; }

    [JsonPropertyName("licence")]
    public string Licence { get; set; }

    [JsonPropertyName("osm_type")]
    public string OsmType { get; set; }

    [JsonPropertyName("osm_id")]
    public long OsmId { get; set; }

    [JsonPropertyName("lat")]
    public string Latitude { get; set; }

    [JsonPropertyName("lon")]
    public string Longitude { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("place_rank")]
    public int PlaceRank { get; set; }

    [JsonPropertyName("importance")]
    public double Importance { get; set; }

    [JsonPropertyName("addresstype")]
    public string AddressType { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }

    [JsonPropertyName("boundingbox")]
    public List<string> BoundingBox { get; set; }

    // [JsonPropertyName("geojson")]
    // public GeoJson GeoJson { get; set; }
}

// public class GeoJson
// {
//     [JsonPropertyName("type")]
//     public string Type { get; set; }

//     [JsonPropertyName("coordinates")]
//     public List<List<List<double>>> Coordinates { get; set; }
// }
