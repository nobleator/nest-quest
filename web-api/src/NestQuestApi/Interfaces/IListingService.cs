using NestQuestApi.Models;

namespace NestQuestApi.Interfaces;

public interface IListingService
{
    public Task<IEnumerable<Listing>> GetListingsByBbox(double minLon, double minLat, double maxLon, double maxLat);
}