

using MemoryLane_D2_MVC.Models;

namespace MemoryLane_D2_MVC.Interfaces
{
    public interface IAlbumRepository
    {
        Task<IEnumerable<Album>> GetByUserIdAsync(string userId);

        Task<Album?> GetByIdAsync(int id, string userId);

        Task<int> CreateAsync(Album album);

        Task<bool> DeleteAsync(int id, string userId);
    }
}
