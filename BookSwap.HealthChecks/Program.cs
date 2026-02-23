using HealthChecks.Redis;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Polly;
using Polly.Extensions.Http;
using StackExchange.Redis;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
    => Policy.TimeoutAsync<HttpResponseMessage>(2);

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger logger) =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .Or<TaskCanceledException>()
        .Or<HttpRequestException>()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 2,
            durationOfBreak: TimeSpan.FromSeconds(15),
            onBreak: (outcome, timespan) =>
                logger.LogWarning("[POLLY] Circuit broken! {Error}",
                    outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()),
            onReset: () => logger.LogInformation("[POLLY] Circuit reset"),
            onHalfOpen: () => logger.LogInformation("[POLLY] Circuit half-open")
        );

static IAsyncPolicy<HttpResponseMessage> GetPolicyWrap(ILogger logger)
    => Policy.WrapAsync(
        GetTimeoutPolicy(),
        GetCircuitBreakerPolicy(logger)
    );

builder.Services.AddHttpClient("aggregator", (sp, client) =>
{
    client.BaseAddress = new Uri("https://localhost:7186/health");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
})
.AddPolicyHandler((sp, request) =>
    GetPolicyWrap(sp.GetRequiredService<ILogger<Program>>()));

var redis = ConnectionMultiplexer.Connect("localhost:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

builder.Services.AddHealthChecks()
    .AddCheck<AggregatorHealthCheck>("aggregator")
    .AddMongoDb(
        sp => new MongoClient(builder.Configuration["ConnectionStrings:MongoDb"]),
        name: "mongodb",
        failureStatus: HealthStatus.Unhealthy
    )
    .AddRedis(
        "localhost:6379,abortConnect=false",
        name: "redis",
        failureStatus: HealthStatus.Unhealthy
    )
    .AddCheck<RedisStreamHealthCheck>("redis-stream");

builder.Services.AddHealthChecksUI(setup =>
{
    setup.SetEvaluationTimeInSeconds(10);
    setup.MaximumHistoryEntriesPerEndpoint(50);
})
.AddInMemoryStorage();

var app = builder.Build();

app.MapGet("/", ctx =>
{
    ctx.Response.Redirect("/health-ui");
    return Task.CompletedTask;
});

app.MapHealthChecks("/health-aggregator", new HealthCheckOptions
{
    Predicate = check => check.Name == "aggregator",
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    AllowCachingResponses = false
});

app.MapHealthChecks("/health-mongodb", new HealthCheckOptions
{
    Predicate = check => check.Name == "mongodb",
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health-redis", new HealthCheckOptions
{
    Predicate = check => check.Name == "redis",
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health-redis-stream", new HealthCheckOptions
{
    Predicate = check => check.Name == "redis-stream",
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
    options.ApiPath = "/health-ui-api";
});

app.MapGet("/test-circuit", async (IHttpClientFactory httpClientFactory, ILogger<Program> logger) =>
{
    var client = httpClientFactory.CreateClient("aggregator");

    try
    {
        var response = await client.GetAsync("");
        logger.LogInformation("[TEST-CIRCUIT] Status code: {StatusCode}", response.StatusCode);
        return Results.Ok($"Status code: {response.StatusCode}");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "[TEST-CIRCUIT] Exception");
        return Results.Problem(ex.Message);
    }
});


app.Run();
