using BookSwap.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookSwap.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<List<UserDocument>> GetAllAsync();
        Task<UserDocument?> GetByIdAsync(string id);
        Task CreateAsync(UserDocument entity);
        Task UpdateAsync(string id, UserDocument entity);
        Task DeleteAsync(string id);
    }
}
