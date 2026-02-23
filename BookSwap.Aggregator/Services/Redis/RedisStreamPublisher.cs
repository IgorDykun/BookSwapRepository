using BookSwap.Aggregator.ViewModels;
using StackExchange.Redis;
using System.Text.Json;

namespace BookSwap.Aggregator.Services
{
    public class RedisStreamPublisher
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisStreamPublisher(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task PublishBookCreatedAsync(BookViewModel book)
        {
            if (book == null || string.IsNullOrEmpty(book.Id))
            {
                Console.WriteLine("Book or Book.Id is empty, skipping Redis stream publish.");
                return;
            }

            var db = _redis.GetDatabase();
            var values = new NameValueEntry[]
            {
                new NameValueEntry("Id", book.Id),
                new NameValueEntry("Title", book.Title ?? string.Empty),
                new NameValueEntry("Author", book.Author ?? string.Empty),
                new NameValueEntry("OwnerId", book.Owner?.Id ?? string.Empty),
                new NameValueEntry("OwnerName", book.Owner?.Name ?? string.Empty)
            };

            var messageId = await db.StreamAddAsync("books-stream", values);
            Console.WriteLine($"Published book {book.Id} to Redis stream 'books-stream' with ID {messageId}");
        }
    }
}
