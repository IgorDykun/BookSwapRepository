using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.MongoDb;
using MongoDB.Driver;

namespace BookSwap.Infrastructure.Repositories
{
    public class BookRepository : BaseMongoRepository<BookDocument>, IBookRepository
    {
        public BookRepository(MongoDbContext context)
            : base(context, "Books") 
        {
        }

        public async Task<List<BookDocument>> GetBooksByAuthorAsync(string author)
        {
            var filter = Builders<BookDocument>.Filter.Eq(b => b.Author, author);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<List<BookDocument>> GetRecentBooksAsync(int days)
        {
            var filter = Builders<BookDocument>.Filter.Gt(b => b.CreatedAt, DateTime.UtcNow.AddDays(-days));
            return await _collection.Find(filter).ToListAsync();
        }

    }
}
