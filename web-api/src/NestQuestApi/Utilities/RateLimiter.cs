namespace NestQuestApi.Utilities;

public class RateLimiter
{
    private readonly SemaphoreSlim _semaphore;
    private readonly TimeSpan _minInterval;
    private DateTime _lastRequestTime = DateTime.MinValue;
    private readonly object _lock = new object();

    public RateLimiter(int maxConcurrentRequests, TimeSpan minIntervalBetweenRequests)
    {
        _semaphore = new SemaphoreSlim(maxConcurrentRequests);
        _minInterval = minIntervalBetweenRequests;
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        await _semaphore.WaitAsync();

        try
        {
            TimeSpan waitTime = TimeSpan.Zero;

            lock (_lock)
            {
                var nextAllowedTime = _lastRequestTime + _minInterval;
                if (DateTime.UtcNow < nextAllowedTime)
                    waitTime = nextAllowedTime - DateTime.UtcNow;

                _lastRequestTime = DateTime.UtcNow + waitTime;
            }

            if (waitTime > TimeSpan.Zero)
                await Task.Delay(waitTime);

            return await action();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
