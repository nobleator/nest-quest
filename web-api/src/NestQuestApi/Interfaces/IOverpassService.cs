using NestQuestApi.Models;

namespace NestQuestApi.Interfaces;

public interface IOverpassService
{
    public Task<IEnumerable<object>> GetPoiByCategoryAndBbox(Category cat, double minLon, double minLat, double maxLon, double maxLat, CancellationToken token);

    public Task<int> GetCountOfMatches(Criterion criterion, double lat, double lon, CancellationToken token);
}