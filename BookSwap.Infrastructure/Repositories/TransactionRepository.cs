using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.MongoDb;

namespace BookSwap.Infrastructure.Repositories
{
    public class TransactionRepository : BaseMongoRepository<TransactionDocument>, ITransactionRepository
    {
        public TransactionRepository(MongoDbContext context)
            : base(context, "Transactions")
        {
        }
    }
}
