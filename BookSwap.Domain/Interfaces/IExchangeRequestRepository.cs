using BookSwap.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookSwap.Domain.Interfaces
{
    public interface IExchangeRequestRepository
    {
        Task<List<ExchangeRequestDocument>> GetAllAsync();
        Task<ExchangeRequestDocument?> GetByIdAsync(string id);
        Task CreateAsync(ExchangeRequestDocument entity);
        Task UpdateAsync(string id, ExchangeRequestDocument entity);
        Task DeleteAsync(string id);
    }
}
