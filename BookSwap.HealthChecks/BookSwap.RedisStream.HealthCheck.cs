using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

public class RedisStreamHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _redis;

    public RedisStreamHealthCheck(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            await db.StreamLengthAsync("books-stream");
            return HealthCheckResult.Healthy("Redis Streams is available");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis Streams unavailable", ex);
        }
    }
}
