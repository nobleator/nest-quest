
using NestQuestApi.Models;
using NestQuestApi.Interfaces;

namespace NestQuestApi.Services;

public class PlaceholderService : IListingService
{
    public async Task<IEnumerable<Listing>> GetListingsByBbox(double minLon, double minLat, double maxLon, double maxLat)
    {
        var homes = new List<Listing>
        {
            new("Big Ben", 51.5052345, -0.09),
            new("Tesco Express", 51.5065, -0.095),
            new("Heathrow Airport", 51.51, -0.1),
            new("British Library", 51.49, -0.08),
            new("London Library", 51.511, -0.09),
            new("Hyde Park", 51.5075, -0.07),
            new("Regent's Park", 51.52, -0.1),
            new("King's College London", 51.52, -0.12),
            new("Sainsbury's Local", 51.49, -0.06),
            new("Waitrose & Partners", 51.52, -0.095),
            new("Random Flat A", 51.48, -0.1),
            new("Green Park", 51.513, -0.11),
            new("Baker Street 221B", 51.515, -0.08),
            new("Camden Town", 51.507, -0.08),
            new("Tower of London", 51.49, -0.11)
        };
        var filteredHomes = homes.Where(h => 
            h.Lat >= minLat && h.Lat <= maxLat &&
            h.Lon >= minLon && h.Lon <= maxLon
        ).ToList();
        return await Task.FromResult(filteredHomes);
    }
}