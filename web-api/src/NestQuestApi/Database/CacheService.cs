using System.Text.Json;

public class CacheService<T>
{
    private readonly ILogger<CacheService<T>> _logger;
    private readonly AppDbContext _dbContext;

    public CacheService(ILogger<CacheService<T>> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<T> GetOrFetchDataAsync(string key, Func<Task<T>> fetchFromSource)
    {
        var cachedEntry = await _dbContext.CacheEntries.FindAsync(key);
        if (cachedEntry != null)
        {
            _logger.LogInformation("Cache hit!");
            return JsonSerializer.Deserialize<T>(cachedEntry.Response);
        }

        _logger.LogInformation("Cache miss. Fetching from source...");
        T result = await fetchFromSource();
        var resultStr = JsonSerializer.Serialize(result);
        _dbContext.CacheEntries.Add(new CacheEntry
        {
            Parameters = key,
            Response = resultStr
        });

        await _dbContext.SaveChangesAsync();
        return result;
    }
}
