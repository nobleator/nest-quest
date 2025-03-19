using Microsoft.EntityFrameworkCore;
using NestQuest.Services;
using OverpassApiModel;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<CacheDbContext>(o => o.UseSqlite("Data Source=cache.db"));
builder.Services.AddScoped<CacheService<OverpassApiResponse>>();
builder.Services.AddTransient<OverpassService>();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CacheDbContext>();
    dbContext.Database.Migrate();
}

app.MapGet("/api/v0/homes", (double minLon, double minLat, double maxLon, double maxLat) => {
    var homes = new List<double[]>
    {
        new[] { 51.505, -0.09 },   // Near first attraction
        new[] { 51.5065, -0.095 }, // Near Grocery
        new[] { 51.51, -0.1 },     // Near Airport
        new[] { 51.49, -0.08 },    // Near Library
        new[] { 51.511, -0.09 },   // Near Library
        new[] { 51.5075, -0.07 },  // Near Park
        new[] { 51.52, -0.1 },     // Near Park
        new[] { 51.52, -0.12 },    // Near School
        new[] { 51.49, -0.06 },    // Near Grocery
        new[] { 51.52, -0.095 },   // Near Grocery
        new[] { 51.48, -0.1 },     // Random
        new[] { 51.513, -0.11 },   // Near Park
        new[] { 51.515, -0.08 },   // Random
        new[] { 51.507, -0.08 },   // Random
        new[] { 51.49, -0.11 }     // Random
    };

    return Results.Ok(homes);
});

app.MapGet("/api/v0/attractions", async (CancellationToken token, OverpassService overpassService, double minLon, double minLat, double maxLon, double maxLat) =>
{
    
    var attractionLocations = new List<object>
    {
        new { location = new[] { 51.506, -0.095 }, type = "Grocery" },
        new { location = new[] { 51.51, -0.08 }, type = "Airport" },
        new { location = new[] { 51.52, -0.09 }, type = "Library" },
        new { location = new[] { 51.507, -0.07 }, type = "Park" },
        new { location = new[] { 51.52, -0.12 }, type = "School" },
        new { location = new[] { 51.49, -0.06 }, type = "Grocery" },
        new { location = new[] { 51.511, -0.1 }, type = "Library" },
        new { location = new[] { 51.513, -0.11 }, type = "Park" }
    };

    return Results.Ok(attractionLocations);
});

app.MapGet("/api/v0/amenities", async (CancellationToken token, OverpassService overpassService, double minLon, double minLat, double maxLon, double maxLat) =>
{
    var amenities = await overpassService.GetDistinctAmenityValues(minLon, minLat, maxLon, maxLat, token);
    Console.WriteLine(string.Join("", amenities));
    // var amenities = await overpassService.GetAmenityValueCounts(minLon, minLat, maxLon, maxLat, token);
    return Results.Ok(amenities);
});

app.Run();
