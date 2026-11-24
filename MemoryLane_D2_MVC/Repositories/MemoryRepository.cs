
using Dapper;
using MemoryLane_D2_MVC.Data;
using MemoryLane_D2_MVC.Interfaces;
using MemoryLane_D2_MVC.Models;

namespace MemoryLane_D2_MVC.Repositories
{
    public class MemoryRepository : IMemoryRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MemoryRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Memory>> GetByUserIdAsync(
            string userId, string? search, string? category)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                SELECT *
                FROM Memories
                WHERE UserId = @UserId
                AND (@Search IS NULL OR Title LIKE '%' + @Search + '%'
                     OR Story LIKE '%' + @Search + '%')
                AND (@Category IS NULL OR Category = @Category)
                ORDER BY MemoryDate DESC";

            return await connection.QueryAsync<Memory>(
                sql,
                new
                {
                    UserId = userId,
                    Search = string.IsNullOrWhiteSpace(search) ? null : search,
                    Category = string.IsNullOrWhiteSpace(category) ? null : category
                });
        }

        public async Task<IEnumerable<string>> GetCategoriesByUserIdAsync(
            string userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                SELECT DISTINCT Category
                FROM Memories
                WHERE UserId = @UserId
                ORDER BY Category";

            return await connection.QueryAsync<string>(
                sql, new { UserId = userId });
        }

        public async Task<Memory?> GetByIdAsync(int id, string userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                SELECT *
                FROM Memories
                WHERE Id = @Id AND UserId = @UserId";

            return await connection.QueryFirstOrDefaultAsync<Memory>(
                sql, new { Id = id, UserId = userId });
        }

        public async Task<int> CreateAsync(Memory memory)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                INSERT INTO Memories
                (Title, Story, MemoryDate, Category, Place, ImagePath,
                 IsFavourite, CreatedAt, UserId, AlbumId)
                VALUES
                (@Title, @Story, @MemoryDate, @Category, @Place, @ImagePath,
                 @IsFavourite, @CreatedAt, @UserId, @AlbumId);

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parameters = new
            {
                memory.Title,
                memory.Story,
                memory.MemoryDate,
                memory.Category,
                memory.Place,
                memory.ImagePath,
                IsFavourite = memory.IsFavourite,
                memory.CreatedAt,
                memory.UserId,
                memory.AlbumId
            };

            return await connection.ExecuteScalarAsync<int>(sql, parameters);
        }

        public async Task<bool> UpdateAsync(Memory memory)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                UPDATE Memories
                SET Title = @Title,
                    Story = @Story,
                    MemoryDate = @MemoryDate,
                    Category = @Category,
                    Place = @Place,
                    ImagePath = @ImagePath,
                    IsFavourite = @IsFavourite,
                    AlbumId = @AlbumId
                WHERE Id = @Id AND UserId = @UserId";

            var parameters = new
            {
                memory.Title,
                memory.Story,
                memory.MemoryDate,
                memory.Category,
                memory.Place,
                memory.ImagePath,
                IsFavourite = memory.IsFavourite,
                memory.AlbumId,
                memory.Id,
                memory.UserId
            };

            int rows = await connection.ExecuteAsync(sql, parameters);

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
                DELETE FROM Memories
                WHERE Id = @Id AND UserId = @UserId";

            int rows = await connection.ExecuteAsync(
                sql, new { Id = id, UserId = userId });

            return rows > 0;
        }
    }
}
