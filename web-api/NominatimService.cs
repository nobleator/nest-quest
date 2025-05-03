using RestSharp;
using NominatimApiModel;

namespace NestQuest.Services;

public class NominatimService
{
    private readonly ILogger<NominatimService> _logger;
    private CacheService<NominatimApiResponse> _cache;
    private readonly RateLimiter _rateLimiter;
    public NominatimService(ILogger<NominatimService> logger, CacheService<NominatimApiResponse> cache, RateLimiter rateLimiter)
    {
        _logger = logger;
        _cache = cache;
        _rateLimiter = rateLimiter;
    }
    
    public async Task<Place> Geocode(string address, CancellationToken token)
    {
        _logger.LogInformation("Making request to Nominatim to geocode address...");
        // var response = new NominatimApiResponse();
        var response = await _cache.GetOrFetchDataAsync(address, async () => {
            return await _rateLimiter.ExecuteAsync(async () => {
                var client = new RestClient();
                var request = new RestRequest("https://nominatim.openstreetmap.org/search.php");
                // request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddQueryParameter("q", address);
                request.AddQueryParameter("polygon_geojson", 1);
                request.AddQueryParameter("format", "jsonv2");
                request.AddQueryParameter("limit", "1");
                _logger.LogInformation("Fetching data...");
                var r = await client.GetAsync(request, token);
                _logger.LogInformation($"Response: {@r.Content}");
                var resp = await client.GetAsync<List<NominatimApiResponse>>(request, token);
                // var test = new NominatimApiResponse();
                // return test;
                return resp.FirstOrDefault();
            });
        });
        _logger.LogInformation($"Request completed: {@response}");
        return NominatimToPlace(response);
    }

    private static Place NominatimToPlace(NominatimApiResponse nominatimModel)
    {
        return new Place
        {
            Name = nominatimModel.Name,
            Address = nominatimModel.DisplayName,
            Latitude = double.TryParse(nominatimModel.Latitude, out var x) ? x : 0,
            Longitude = double.TryParse(nominatimModel.Longitude, out var y) ? y : 0
        };
    }
}