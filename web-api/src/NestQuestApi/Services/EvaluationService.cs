using NestQuestApi.Models;
using NestQuestApi.Interfaces;

namespace NestQuestApi.Services;

public class EvaluationService : IEvaluationService
{
    private readonly ILogger<EvaluationService> _logger;
    private IOverpassService _overpassService;

    public EvaluationService(ILogger<EvaluationService> logger, IOverpassService overpassService)
    {
        _logger = logger;
        _overpassService = overpassService;
    }
    
    public async Task<double> BinaryScore(double lat, double lon, IEnumerable<Criterion> criteria, CancellationToken token)
    {
        // return await Task.FromResult(true);
        // TODO: pass/fail boolean result if criteria are met or not
        _logger.LogInformation($"BinaryScore evaluation...");
        var tasks = criteria.Select(c => _overpassService.GetCountOfMatches(c, lat, lon, token));
        var results = await Task.WhenAll(tasks);
        _logger.LogInformation($"BinaryScore evaluation completed, results: {string.Join(", ", results)}");
        return results.All(count => count > 0) ? 1 : 0;
    }

    public async Task<double> ContinuousScore(double lat, double lon, IEnumerable<Criterion> criteria, CancellationToken token)
    {
        // TODO: add weights, add count vs min/max logic, normalize results to a 0-1 range
        return await Task.FromResult(1.0);
    }

    public async Task<Dictionary<int, int>> BinaryScoreDetail(double lat, double lon, IEnumerable<Criterion> criteria, CancellationToken token)
    {
        _logger.LogInformation($"BinaryScore evaluation...");
        var tasks = criteria.ToDictionary(
            c => c.Id,
            c => _overpassService.GetCountOfMatches(c, lat, lon, token)
        );
        var results = await Task.WhenAll(tasks.Values);
        var resultDictionary = criteria.Zip(results, (c, result) => new { c, result })
                               .ToDictionary(x => x.c.Id, x => x.result);
        _logger.LogInformation($"BinaryScore evaluation completed, results: {string.Join(Environment.NewLine, resultDictionary.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");
        return resultDictionary;
    }
}