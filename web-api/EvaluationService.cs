namespace NestQuest.Services;

public class EvaluationService
{
    private OverpassService _overpassService;

    public EvaluationService(OverpassService overpassService)
    {
        _overpassService = overpassService;
    }
    
    public async Task<double> BinaryScore(double lat, double lon, IEnumerable<Criterion> criteria, CancellationToken token)
    {
        // return await Task.FromResult(true);
        // TODO: pass/fail boolean result if criteria are met or not
        Console.WriteLine($"BinaryScore evaluation...");
        var tasks = criteria.Select(c => _overpassService.GetCountOfMatches(c, lat, lon, token));
        var results = await Task.WhenAll(tasks);
        Console.WriteLine($"BinaryScore evaluation completed, results: {string.Join(", ", results)}");
        return results.All(count => count > 0) ? 1 : 0;
    }

    public async Task<double> ContinuousScore(double lat, double lon, IEnumerable<Criterion> criteria, CancellationToken token)
    {
        // TODO: add weights, add count vs min/max logic, normalize results to a 0-1 range
        return await Task.FromResult(1.0);
    }
}