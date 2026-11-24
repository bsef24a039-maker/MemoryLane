
using MemoryLane_D2_MVC.Models;

namespace MemoryLane_D2_MVC.Interfaces
{
    public interface IUserRepository
    {
        Task<int> GetUserCountAsync();

        Task<IEnumerable<ApplicationUser>> GetRecentUsersAsync(int count);
    }
}
