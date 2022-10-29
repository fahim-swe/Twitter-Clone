using System.Text.Json;
using core.Entities;
using core.Interfaces.Redis;
using StackExchange.Redis;

namespace infrastructure.Services.Redis
{
    public class BlockUserRedisRepository : IBlockUserRedisRepository
    {
        
        private readonly IDatabase _database;
        public BlockUserRedisRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task AddBlockUserAsync(AdminBlock user)
        {
            var created = await _database.StringSetAsync("AdminBlock"+user.UserId, 
                JsonSerializer.Serialize(user));
            return ;
        }

        public async Task<bool> DeleteBlockUserAsync(string userId)
        {
            return await _database.KeyDeleteAsync("AdminBlock"+userId);
        }

        public async Task<AdminBlock> GetBlockUserAsync(string userId)
        {
            var data = await _database.StringGetAsync("AdminBlock"+userId);
            return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<AdminBlock>(data);
        }
    }
}