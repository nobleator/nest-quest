using Models;

namespace NestQuest.Services;

public interface IListingService
{
    public Task<IEnumerable<Listing>> GetListingsByBbox(double minLon, double minLat, double maxLon, double maxLat);
}