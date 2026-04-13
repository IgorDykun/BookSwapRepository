using BookSwap.Aggregator;
using BookSwap.Aggregator.Data;
using BookSwap.Aggregator.Data; 
using BookSwap.Aggregator.Models;
using BookSwap.Aggregator.Services;
using BookSwap.Infrastructure.MongoDb;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using StackExchange.Redis;
using System;


var builder = WebApplication.CreateBuilder(args);

// 1. Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddInfrastructure(builder.Configuration);

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
               .AddHttpClientInstrumentation()
               .AddOtlpExporter();
    });

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=user_profiles.db"));


builder.Services.AddSingleton<ReminderParser>();
builder.Services.AddHostedService<ReminderWorker>();
builder.Services.AddScoped<CalendarManager>();

var app = builder.Build();

// 5. Pipeline

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