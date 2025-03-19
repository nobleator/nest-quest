using RestSharp;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Web;
using OverpassApiModel;

namespace NestQuest.Services;

/*
[out:json][timeout:25];
node["amenity"](bbox=-0.489,51.28,0.236,51.686);
out;

[out:json][timeout:25];
node(51.28,-0.489,51.686,0.236);
out;
*/

public class OverpassService
{
    /*
    TODO: map OSM tags and values to search terms
    E.g., the search term "library" should map to "amenity"="library", while "park" maps to "leisure"="park"
    */
    private CacheService<OverpassApiResponse> _cache;
    public OverpassService(CacheService<OverpassApiResponse> cache)
    {
        _cache = cache;
    }
    
    public async Task<IEnumerable<string>> GetDistinctAmenityValues(double minLon, double minLat, double maxLon, double maxLat, CancellationToken token)
    {
        Console.WriteLine("Making request for Overpass data...");
        var query = ToOverpassQL(minLon, minLat, maxLon, maxLat);
        var response = await _cache.GetOrFetchDataAsync(query, async () => {
            var client = new RestClient();
            var request = new RestRequest("https://www.overpass-api.de/api/interpreter");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("data", query);
            Console.WriteLine("Fetching data...");
            return await client.PostAsync<OverpassApiResponse>(request, token);
        });
        
        Console.WriteLine("Request completed.");
        Console.WriteLine(response);
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(response, options);
        Console.WriteLine(json);
        var amenities = response.Elements
            .Select(x => x.Tags)
            .Where(x => x.ContainsKey("amenity"))
            .SelectMany(x => x.Values.ToList())
            .Distinct();
        return amenities;
    }

    // public async Task<IDictionary<string, int>> GetAmenityValueCounts(double minLon, double minLat, double maxLon, double maxLat, CancellationToken token)
    // {
    //     Console.WriteLine("Making request to Overpass API...");
    //     var client = new RestClient();
    //     var request = new RestRequest("https://www.overpass-api.de/api/interpreter");
    //     request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
    //     request.AddParameter("data", ToOverpassQL(minLon, minLat, maxLon, maxLat));
    //     Console.WriteLine("Making request...");
        
    //     var response = await client.PostAsync<OverpassApiResponse>(request, token);
    //     Console.WriteLine("Request completed.");
    //     Console.WriteLine(response);
    //     var options = new JsonSerializerOptions { WriteIndented = true };
    //     string json = JsonSerializer.Serialize(response, options);
    //     Console.WriteLine(json);
    //     var amenityCounts = response.Elements
    //         .Select(x => x.Tags)
    //         .Where(x => x.ContainsKey("amenity"))
    //         .SelectMany(x => x.Values)
    //         .GroupBy(x => x)
    //         .ToDictionary(g => g.Key, g => g.Count());
    //     return amenityCounts;
    // }

    private string ToOverpassQL(double minLon, double minLat, double maxLon, double maxLat) 
    {
        Console.WriteLine("Building OverpassQL statement...");
        var input = $"[out:json];node[amenity=drinking_water]({minLat},{minLon},{maxLat},{maxLon});out;";
        // var encoded = HttpUtility.UrlEncode(input);
        // Console.WriteLine("Encoding completed.");
        // Console.WriteLine(encoded);
        return input;
    }
}