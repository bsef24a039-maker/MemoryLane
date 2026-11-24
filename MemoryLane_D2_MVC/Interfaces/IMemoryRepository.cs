
using MemoryLane_D2_MVC.Models;

namespace MemoryLane_D2_MVC.Interfaces
{
    public interface IMemoryRepository
    {
        Task<IEnumerable<Memory>> GetByUserIdAsync(
            string userId, string? search, string? category);

        Task<IEnumerable<string>> GetCategoriesByUserIdAsync(string userId);

        Task<Memory?> GetByIdAsync(int id, string userId);

        Task<int> CreateAsync(Memory memory);

        Task<bool> UpdateAsync(Memory memory);

        Task<bool> DeleteAsync(int id, string userId);
    }
}
