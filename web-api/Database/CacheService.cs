using System.Text.Json;

public class CacheService<T>
{
    private readonly CacheDbContext _dbContext;

    public CacheService(CacheDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T> GetOrFetchDataAsync(string key, Func<Task<T>> fetchFromSource)
    {
        var cachedEntry = await _dbContext.CacheEntries.FindAsync(key);
        if (cachedEntry != null)
        {
            Console.WriteLine("Cache hit!");
            return JsonSerializer.Deserialize<T>(cachedEntry.Response);
        }

        Console.WriteLine("Cache miss. Fetching from source...");
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
