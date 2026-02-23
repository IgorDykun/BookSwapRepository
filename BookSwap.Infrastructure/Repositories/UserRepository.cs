using BookSwap.Domain.Entities;
using BookSwap.Domain.Interfaces;
using BookSwap.Infrastructure.MongoDb;

namespace BookSwap.Infrastructure.Repositories
{
    public class UserRepository : BaseMongoRepository<UserDocument>, IUserRepository
    {
        public UserRepository(MongoDbContext context)
            : base(context, "Users")
        {
        }
    }
}
