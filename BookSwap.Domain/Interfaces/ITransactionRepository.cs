using BookSwap.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookSwap.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<TransactionDocument>> GetAllAsync();
        Task<TransactionDocument?> GetByIdAsync(string id);
        Task CreateAsync(TransactionDocument entity);
        Task UpdateAsync(string id, TransactionDocument entity);
        Task DeleteAsync(string id);
    }
}
