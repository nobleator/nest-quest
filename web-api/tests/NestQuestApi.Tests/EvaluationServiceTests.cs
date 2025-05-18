using Moq;
using Microsoft.Extensions.Logging;
using NestQuestApi.Interfaces;
using NestQuestApi.Services;
using NestQuestApi.Models;

namespace NestQuestApi.Tests;

public class EvaluationServiceTests
{
    [Fact]
    public async Task BinaryScore_ShouldReturn1()
    {
        var mockLogger = new Mock<ILogger<EvaluationService>>();
        var mockOverpassService = new Mock<IOverpassService>();
        mockOverpassService.Setup(s => s.GetCountOfMatches(It.IsAny<Criterion>(), It.IsAny<double>(), It.IsAny<double>(), CancellationToken.None)).ReturnsAsync(1);

        var mockEvalService = new EvaluationService(mockLogger.Object, mockOverpassService.Object);
        var result = await mockEvalService.BinaryScore(1, 1, [], CancellationToken.None);

        Assert.Equal(1, result);
    }
}
