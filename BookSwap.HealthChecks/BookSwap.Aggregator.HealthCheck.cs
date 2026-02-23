using Microsoft.Extensions.Diagnostics.HealthChecks;

public class AggregatorHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AggregatorHealthCheck> _logger;

    public AggregatorHealthCheck(IHttpClientFactory httpClientFactory, ILogger<AggregatorHealthCheck> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("[HC] Running AggregatorHealthCheck…");

            var client = _httpClientFactory.CreateClient("aggregator");
            var response = await client.GetAsync("", cancellationToken);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogInformation("[HC] Aggregator responded: {StatusCode}, body: {Content}", (int)response.StatusCode, content);

            var data = new Dictionary<string, object>
            {
                { "statusCode", (int)response.StatusCode },
                { "response", content }
            };

            if (response.IsSuccessStatusCode)
                return HealthCheckResult.Healthy("Aggregator is healthy", data);

            return HealthCheckResult.Unhealthy("Aggregator returned error", null, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[HC] AggregatorHealthCheck exception");

            var data = new Dictionary<string, object>
            {
                { "exception", ex.Message }
            };

            return HealthCheckResult.Unhealthy("Aggregator check failed", ex, data);
        }
    }
}
