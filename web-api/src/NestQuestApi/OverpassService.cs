using RestSharp;
using System.Text.Json;
using OverpassApiModel;
using POI = PointOfInterest;
using Criteria;

namespace NestQuest.Services;

/*
These examples can be run via Overpass turbo (https://overpass-turbo.eu/).

https://dev.overpass-api.de/overpass-doc/en/criteria/union.html

The following query is equivalent to the English sentence:
"Return all nodes within the provided bounding box in JSON format where the node contains a tag of amentity=cafe or leisure=park."
This uses an OR syntax via a UNION (defined via `(...)`):
```
[out:json];
(
  node[amenity=cafe]({{bbox}});
  node[leisure=park]({{bbox}});
);
out;
```

The following query is equivalent to the English sentence:
"Return all nodes within the provided bounding box in JSON format where the node contains tags of amentity=cafe and cuisine=coffee_shop."
This uses an AND syntax:
```
[out:json];
node[amenity=cafe][cuisine=coffee_shop]({{bbox}});
out;
```

The following query is equivalent to the English sentence:
"Return all nodes within the provided bounding box in JSON format where the node either 1) contains tags of amentity=cafe and cuisine=coffee_shop or 2) contains the tag leisure=park."
This uses both AND and OR syntax combined:
```
[out:json];
(
  node[amenity=cafe][cuisine=coffee_shop]({{bbox}});
  node[leisure=park]({{bbox}});
);
out;
```

When running programmatically, i.e. not via overpass turbo, replace the {{bbox}} placeholder with latitude and longitude values like so:
```
[out:json];
node["amenity"](bbox=-0.489,51.28,0.236,51.686);
out;
```

(TBD) It is also generally recommended to provide a timeout setting to ensure accidentally huge results fail gracefully:
```
[out:json][timeout:10];
node["amenity"](bbox=-0.489,51.28,0.236,51.686);
out;
```
*/

public class OverpassService
{
    private readonly ILogger<OverpassService> _logger;
    private CacheService<OverpassApiResponse> _cache;
    private readonly RateLimiter _rateLimiter;
    public OverpassService(ILogger<OverpassService> logger, CacheService<OverpassApiResponse> cache, RateLimiter rateLimiter)
    {
        _logger = logger;
        _cache = cache;
        _rateLimiter = rateLimiter;
    }
    
    public async Task<IEnumerable<object>> GetPoiByCategoryAndBbox(POI.Category cat, double minLon, double minLat, double maxLon, double maxLat, CancellationToken token)
    {
        // _logger.LogInformation("Making request for Overpass data...");
        var query = $"[out:json];node{GetTagsForCategory(cat)}({minLat},{minLon},{maxLat},{maxLon});out;";
        var response = await _cache.GetOrFetchDataAsync(query, async () => {
            return await _rateLimiter.ExecuteAsync(async () => {
                var client = new RestClient();
                var request = new RestRequest("https://www.overpass-api.de/api/interpreter");
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("data", query);
                // _logger.LogInformation("Fetching data...");
                return await client.PostAsync<OverpassApiResponse>(request, token);
            });
        });
        // _logger.LogInformation("Request completed.");
        // _logger.LogInformation(response);
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(response, options);
        // _logger.LogInformation(json);
        var poiLocations = response.Elements
            .Select(e => new { location = new[] { e.Lat, e.Lon }, type = cat.ToString() });
      return poiLocations;
    }

    public async Task<int> GetCountOfMatches(Criterion criterion, double lat, double lon, CancellationToken token)
    {
        _logger.LogInformation("Making request for Overpass data...");
        var query = $"[out:json];node{GetTagsForCategory(criterion.Category)}(around:{criterion.Tolerance},{lat},{lon});out count;";
        _logger.LogInformation(query);
        var response = await _cache.GetOrFetchDataAsync(query, async () => {
            return await _rateLimiter.ExecuteAsync(async () => {
                var client = new RestClient();
                var request = new RestRequest("https://www.overpass-api.de/api/interpreter");
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("data", query);
                _logger.LogInformation("Fetching data...");
                return await client.PostAsync<OverpassApiResponse>(request, token);
            });
        });
        _logger.LogInformation($"Request completed: {@response}");
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(response, options);
        _logger.LogInformation(json);
        _logger.LogInformation(string.Join(", ", response.Elements.First().Tags.Select(kvp => $"{kvp.Key}: {kvp.Value}")));
        return Convert.ToInt32(response.Elements.First().Tags["total"]);
    }

    private static string GetTagsForCategory(POI.Category category) => category switch
    {
        POI.Category.Park => "[leisure=park]",
        POI.Category.Library => "[amenity=library]",
        POI.Category.School => "[amenity=school]",
        POI.Category.Grocery => "[shop=supermarket]",
        POI.Category.CoffeeShop => "[amenity=cafe][cuisine=coffee_shop]",
        POI.Category.Airport => "[aeroway=terminal]",
        POI.Category.TrainStation => "[building=train_station]",
        POI.Category.BusStation => "[amenity=bus_station]",
        POI.Category.PoliceStation => "[amenity=police]",
        POI.Category.FireStation => "[amenity=fire_station]",
        _ => "TODO"
    };
}