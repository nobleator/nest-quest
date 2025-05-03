using Criteria;
using Microsoft.EntityFrameworkCore;
using Serilog;
using NestQuest.Services;
using OverpassApiModel;
using POI = PointOfInterest;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile($"appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite(connString));
builder.Services.AddScoped<CacheService<OverpassApiResponse>>();
builder.Services.AddTransient<OverpassService>();
builder.Services.AddTransient<EvaluationService>();
builder.Services.AddSingleton(new RateLimiter(1, TimeSpan.FromSeconds(1)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var logFilePath = Environment.GetEnvironmentVariable("LOG_FILE_PATH") ?? "/logs/app.log";
var logLevel = Environment.GetEnvironmentVariable("LOG_LEVEL") ?? "Information";
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is(Enum.Parse<Serilog.Events.LogEventLevel>(logLevel, true))
    .WriteTo.Console()
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(); 

app.MapGet("/api/v0/homes", (double minLon, double minLat, double maxLon, double maxLat) => {
    var homes = new List<POI.Home>
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

    return Results.Ok(filteredHomes);
});

// /api/v0/poi?cat=Park&minLat=51.47528888311576&maxLat=51.53459069801548&minLon=-0.1544952392578125&maxLon=-0.025577545166015625
app.MapGet("/api/v0/poi", async (CancellationToken token, OverpassService overpassService, POI.Category cat, double minLon, double minLat, double maxLon, double maxLat) =>
{
    var poi = await overpassService.GetPoiByCategoryAndBbox(cat, minLon, minLat, maxLon, maxLat, token);
    return Results.Ok(poi);
});

app.MapGet("/api/v0/criteria", async (CancellationToken token, AppDbContext dbContext) =>
{
    var criteria = await dbContext.Criteria.ToListAsync(token);
    return Results.Ok(criteria);
});

// curl 'https://localhost:5001/api/v0/criteria' -X POST -H 'Content-Type: application/json' -d '{"criteria":[{"id":1, "category":1, "tolerance":1, "unit":0, "direction":0}, {"id":2, "category":2, "tolerance":2, "unit":0, "direction":0}]}'
app.MapPost("/api/v0/criteria", async (CancellationToken token, AppDbContext dbContext, CriteriaModel dto) =>
{
    if (dto == null || dto.Criteria.Count == 0)
        return Results.BadRequest("Criteria list is empty or null.");

    foreach (var criterion in dto.Criteria)
    {
        var existingCriterion = await dbContext.Criteria.FindAsync(criterion.Id, token);
        if (existingCriterion != null)
            dbContext.Entry(existingCriterion).CurrentValues.SetValues(criterion);
        else
            dbContext.Criteria.Add(criterion);
    }
    await dbContext.SaveChangesAsync(token);
    var ids = dto.Criteria.Select(c => c.Id).ToList();
    var updatedCriteria = await dbContext.Criteria
        .Where(c => ids.Contains(c.Id))
        .ToListAsync(token);
    return Results.Ok(updatedCriteria);
});

app.MapGet("/api/v0/score", async (ILogger<Program> logger, CancellationToken token, AppDbContext dbContext, EvaluationService evaluationService, double lat, double lon) =>
{
    var criteria = await dbContext.Criteria.ToListAsync();
    logger.LogInformation("Loading saved criteria...");
    logger.LogInformation(string.Join(", ", criteria));
    var score = await evaluationService.BinaryScore(lat, lon, criteria, token);
    return Results.Ok(score);
});

app.MapGet("/api/v0/score-detail", async (ILogger<Program> logger, CancellationToken token, AppDbContext dbContext, EvaluationService evaluationService, double lat, double lon) =>
{
    var criteria = await dbContext.Criteria.ToListAsync();
    logger.LogInformation("Loading saved criteria...");
    logger.LogInformation(string.Join(", ", criteria));
    var score = await evaluationService.BinaryScoreDetail(lat, lon, criteria, token);
    return Results.Ok(score);
});

app.MapGet("/places", async (ILogger<Program> logger, AppDbContext dbContext) =>
{
    logger.LogInformation("Fetching all saved places");
    var places = await dbContext.Places.ToListAsync();
    return Results.Ok(places);
});

app.MapGet("/places/{id:int}", async (int id, ILogger<Program> logger, AppDbContext dbContext) =>
{
    var place = await dbContext.Places.FirstOrDefaultAsync(p => p.Id == id);
    if (place is null)
    {
        logger.LogWarning("Place with ID {Id} not found", id);
        return Results.NotFound($"Place with ID {id} not found.");
    }
    logger.LogInformation("Fetched place with ID {Id}", id);
    return Results.Ok(place);
});

app.MapPost("/places", async (Place newPlace, ILogger<Program> logger, AppDbContext dbContext) =>
{
    var places = await dbContext.Places.ToListAsync();
    newPlace.Id = places.Count > 0 ? places.Max(p => p.Id) + 1 : 1;
    places.Add(newPlace);
    await dbContext.AddAsync(newPlace);
    await dbContext.SaveChangesAsync();
    logger.LogInformation("Added new place with ID {Id}", newPlace.Id);
    return Results.Created($"/places/{newPlace.Id}", newPlace);
});

app.MapPut("/places/{id:int}", async (int id, Place updatedPlace, ILogger<Program> logger, AppDbContext dbContext) =>
{
    var places = await dbContext.Places.ToListAsync();
    var existingPlace = places.FirstOrDefault(p => p.Id == id);
    if (existingPlace is null)
    {
        logger.LogWarning("Attempt to update non-existent place with ID {Id}", id);
        return Results.NotFound($"Place with ID {id} not found.");
    }
    existingPlace.Name = updatedPlace.Name;
    existingPlace.Address = updatedPlace.Address;
    existingPlace.Latitude = updatedPlace.Latitude;
    existingPlace.Longitude = updatedPlace.Longitude;
    dbContext.Places.Update(existingPlace);
    await dbContext.SaveChangesAsync();
    logger.LogInformation("Updated place with ID {Id}", id);
    return Results.Ok(existingPlace);
});

app.MapDelete("/places/{id:int}", async (int id, ILogger<Program> logger, AppDbContext dbContext) =>
{
    var place = await dbContext.Places.FirstOrDefaultAsync(p => p.Id == id);
    if (place is null)
    {
        logger.LogWarning("Attempt to delete non-existent place with ID {Id}", id);
        return Results.NotFound($"Place with ID {id} not found.");
    }
    dbContext.Places.Remove(place);
    await dbContext.SaveChangesAsync();
    logger.LogInformation("Deleted place with ID {Id}", id);
    return Results.NoContent();
});

app.Run();
Log.CloseAndFlush();