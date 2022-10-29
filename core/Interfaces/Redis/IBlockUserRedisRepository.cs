using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities;

namespace core.Interfaces.Redis
{
    public interface IBlockUserRedisRepository
    {
        Task<AdminBlock> GetBlockUserAsync(string userId);
        Task<bool> DeleteBlockUserAsync(string userId);
        Task AddBlockUserAsync(AdminBlock user);
    }
}