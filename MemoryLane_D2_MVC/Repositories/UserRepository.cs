
using Dapper;
using MemoryLane_D2_MVC.Data;
using MemoryLane_D2_MVC.Interfaces;
using MemoryLane_D2_MVC.Models;

namespace MemoryLane_D2_MVC.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> GetUserCountAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "SELECT COUNT(*) FROM AspNetUsers";

            return await connection.ExecuteScalarAsync<int>(sql);
        }

        public async Task<IEnumerable<ApplicationUser>> GetRecentUsersAsync(int count)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                SELECT Id, UserName, Email, FullName
                FROM AspNetUsers
                ORDER BY Id DESC
                OFFSET 0 ROWS FETCH NEXT @Count ROWS ONLY";

            return await connection.QueryAsync<ApplicationUser>(
                sql, new { Count = count });
        }
    }
}
