
using System.Data;

namespace MemoryLane_D2_MVC.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
