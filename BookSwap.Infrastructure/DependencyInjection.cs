using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.MongoDb;
using BookSwap.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));

        services.AddSingleton<MongoDbContext>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoDbSettings>>();
            if (string.IsNullOrEmpty(options.Value.ConnectionString))
            {
                throw new InvalidOperationException("CRITICAL: MongoDb:ConnectionString is missing in configuration!");
            }
            return new MongoDbContext(options.Value);
        });

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IExchangeRequestRepository, ExchangeRequestRepository>();

        return services;
    }
}