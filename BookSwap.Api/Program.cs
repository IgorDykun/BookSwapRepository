using BookSwap.Api.Middleware;
using BookSwap.Application.Users.Commands.CreateUser; 
using BookSwap.Infrastructure;
using BookSwap.GrpcContracts;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Serilog;
using System.Reflection;




var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// MongoDB
builder.Services.AddInfrastructure(builder.Configuration);

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// MediatR 
builder.Services.AddMediatR(typeof(CreateUserCommandHandler).Assembly);

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserCommandHandler>();

// Redis caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"];
    options.InstanceName = "";
});


// Controllers та Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGrpcClient<BooksService.BooksServiceClient>(options =>
{
    options.Address = new Uri("http://localhost:5069");
});


var app = builder.Build();

// Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Запуск застосунку BookSwap...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Застосунок аварійно завершився");
}
finally
{
    Log.CloseAndFlush();
}
