using MongoDB.Driver;

namespace BookSwap.Infrastructure.MongoDb
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(MongoDbSettings settings)
        {
            Console.WriteLine($"MongoDB ConnectionString: {settings.ConnectionString}");
            Console.WriteLine($"MongoDB DatabaseName: {settings.DatabaseName}");

            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }


        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}
