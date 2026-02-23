using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using BookSwap.Infrastructure.MongoDb;
using BookSwap.Aggregator.Services; // Додано для RedisStreamPublisher
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// 1. Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// 2. Підключення шару Infrastructure (БД та Репозиторії)
// Тут реєструються ваші MongoDbContext, IUserRepository, IBookRepository тощо.
builder.Services.AddInfrastructure(builder.Configuration);

// 3. Redis та Publisher (ВИПРАВЛЕНО)
// Перевіряємо різні варіанти назв у конфігурації (Redis__Configuration для Docker або Redis:Configuration для локалу)
var redisConnectionString = builder.Configuration["Redis__Configuration"]
                            ?? builder.Configuration["Redis:Configuration"]
                            ?? "localhost:6379";

// Реєструємо клієнт Redis (необхідно для RedisStreamPublisher)
var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);

// Реєструємо сервіс публікації івентів (необхідно для BookAggregatorController)
builder.Services.AddSingleton<RedisStreamPublisher>();

// Реєструємо кеш (для стандартних потреб ASP.NET Core)
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = redisConnectionString;
});

// 4. OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => {
        tracing.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("BookSwap.Aggregator"))
               .AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation() // Корисно для відстеження зовнішніх викликів
               .AddOtlpExporter();
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// 5. Pipeline
// Swagger завжди доступний на корені (/) для зручності розробки
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookSwap API v1");
    c.RoutePrefix = string.Empty;
});

// Middleware
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();