using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

public class MongoDbHealthCheck : IHealthCheck
{
    private readonly IMongoClient _mongoClient;

    public MongoDbHealthCheck(IMongoClient mongoClient)
    {
        _mongoClient = mongoClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var databaseNames = await _mongoClient.ListDatabaseNamesAsync(cancellationToken);
            var dbList = await databaseNames.ToListAsync(cancellationToken);

            var data = new Dictionary<string, object>
            {
                { "databases", dbList }
            };

            return HealthCheckResult.Healthy("MongoDB is healthy", data);
        }
        catch (Exception ex)
        {
            var data = new Dictionary<string, object>
            {
                { "exception", ex.Message }
            };
            return HealthCheckResult.Unhealthy("MongoDB check failed", ex, data);
        }
    }
}
