
using Dapper;
using MemoryLane_D2_MVC.Data;
using MemoryLane_D2_MVC.Interfaces;
using MemoryLane_D2_MVC.Models;

namespace MemoryLane_D2_MVC.Repositories
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AlbumRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Album>> GetByUserIdAsync(string userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                SELECT *
                FROM Albums
                WHERE UserId = @UserId
                ORDER BY Id DESC";

            return await connection.QueryAsync<Album>(
                sql, new { UserId = userId });
        }

        public async Task<Album?> GetByIdAsync(int id, string userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                SELECT *
                FROM Albums
                WHERE Id = @Id AND UserId = @UserId";

            return await connection.QueryFirstOrDefaultAsync<Album>(
                sql, new { Id = id, UserId = userId });
        }

        public async Task<int> CreateAsync(Album album)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                INSERT INTO Albums (Name, Description, UserId)
                VALUES (@Name, @Description, @UserId);

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await connection.ExecuteScalarAsync<int>(sql, album);
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                DELETE FROM Albums
                WHERE Id = @Id AND UserId = @UserId";

            int rows = await connection.ExecuteAsync(
                sql, new { Id = id, UserId = userId });

            return rows > 0;
        }
    }
}
