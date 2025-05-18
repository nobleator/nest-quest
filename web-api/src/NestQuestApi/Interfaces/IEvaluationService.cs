using NestQuestApi.Models;

namespace NestQuestApi.Interfaces;

public interface IEvaluationService
{
    public Task<double> BinaryScore(double lat, double lon, IEnumerable<Criterion> criteria, CancellationToken token);
    public Task<double> ContinuousScore(double lat, double lon, IEnumerable<Criterion> criteria, CancellationToken token);
    public Task<Dictionary<int, int>> BinaryScoreDetail(double lat, double lon, IEnumerable<Criterion> criteria, CancellationToken token);
}