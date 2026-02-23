using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.MongoDb;

namespace BookSwap.Infrastructure.Repositories
{
    public class ExchangeRequestRepository : BaseMongoRepository<ExchangeRequestDocument>, IExchangeRequestRepository
    {
        public ExchangeRequestRepository(MongoDbContext context)
            : base(context, "ExchangeRequests")
        {
        }
    }
}
