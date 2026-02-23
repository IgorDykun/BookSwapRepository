using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace BookSwap.Infrastructure.MongoDb
{
    public static class MongoDbExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new MongoDbSettings();
            configuration.GetSection("MongoDb").Bind(settings);

            services.AddSingleton(settings);
            services.AddSingleton<MongoDbContext>();

            services.AddSingleton<IBookRepository, BookRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IExchangeRequestRepository, ExchangeRequestRepository>();
            services.AddSingleton<ITransactionRepository, TransactionRepository>();

            return services;
        }
    }
}
