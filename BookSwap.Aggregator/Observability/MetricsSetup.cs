using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics.Metrics;
using OpenTelemetry.Exporter.Prometheus;

namespace BookSwap.Aggregator.Observability
{
    public static class MetricsSetup
    {
        private static readonly Meter AggregatorMeter = new Meter("BookSwap.Aggregator.Metrics", "1.0.0");

        public static readonly Counter<long> BooksRequested = AggregatorMeter.CreateCounter<long>("books_requested_total", "count", "Кількість запитів книг");
        public static readonly Counter<long> BooksCreated = AggregatorMeter.CreateCounter<long>("books_created_total", "count", "Кількість створених книг");
        public static readonly Counter<long> BooksUpdated = AggregatorMeter.CreateCounter<long>("books_updated_total", "count", "Кількість оновлених книг");
        public static readonly Counter<long> BooksDeleted = AggregatorMeter.CreateCounter<long>("books_deleted_total", "count", "Кількість видалених книг");

        public static readonly Histogram<double> GrpcLatency = AggregatorMeter.CreateHistogram<double>("grpc_latency_ms", "ms", "Час відповіді gRPC");
        public static readonly Histogram<double> HttpLatency = AggregatorMeter.CreateHistogram<double>("http_latency_ms", "ms", "Час відповіді HTTP");

        public static readonly ObservableGauge<long> CacheItemsGauge = AggregatorMeter.CreateObservableGauge<long>(
            "cache_items_count",
            () => 0,
            description: "Кількість елементів в кеші"
        );

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddOpenTelemetry()
                .WithTracing(tracerProviderBuilder =>
                {
                    tracerProviderBuilder
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("BookSwap.Aggregator"))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddGrpcClientInstrumentation()
                        .AddConsoleExporter();
                })
                .WithMetrics(meterProviderBuilder =>
                {
                    meterProviderBuilder
                        .AddMeter(AggregatorMeter.Name)
                        .AddRuntimeInstrumentation()
                        .AddHttpClientInstrumentation() 
                        .AddPrometheusExporter();       
                });
        }

        public static void UseMetrics(WebApplication app)
        {
            app.MapPrometheusScrapingEndpoint("/metrics");

            app.Use(async (context, next) =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                await next();
                sw.Stop();
                HttpLatency.Record(sw.Elapsed.TotalMilliseconds);
            });
        }

        public static async Task<T> TrackGrpcCallAsync<T>(Func<Task<T>> action, string methodName)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var result = await action();
                BooksRequested.Add(1, new KeyValuePair<string, object?>("method", methodName));
                return result;
            }
            finally
            {
                sw.Stop();
                GrpcLatency.Record(sw.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("method", methodName));
            }
        }

        public static void IncrementCreated() => BooksCreated.Add(1);
        public static void IncrementUpdated() => BooksUpdated.Add(1);
        public static void IncrementDeleted() => BooksDeleted.Add(1);
    }
}
