using NestQuestApi.Models;
using NestQuestApi.Interfaces;

namespace NestQuestApi.Services;

public class ZillowService : IListingService
{
    public async Task<IEnumerable<Listing>> GetListingsByBbox(double minLon, double minLat, double maxLon, double maxLat)
    {
        // PUT https://www.zillow.com/async-create-search-page-state
        return await Task.FromResult<IEnumerable<Listing>>([]);
    }
}