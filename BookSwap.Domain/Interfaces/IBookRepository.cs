using BookSwap.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookSwap.Domain.Interfaces
{
    public interface IBookRepository
    {
        Task<List<BookDocument>> GetAllAsync();
        Task<BookDocument?> GetByIdAsync(string id);
        Task CreateAsync(BookDocument entity);
        Task UpdateAsync(string id, BookDocument entity);
        Task DeleteAsync(string id);
    }
}
