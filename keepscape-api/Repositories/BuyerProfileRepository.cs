using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.Repositories.Generics;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class BuyerProfileRepository : BaseRepository<BuyerProfile>, IProfileRepository<BuyerProfile>
    {
        public BuyerProfileRepository(APIDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<BuyerProfile>> GetAllAsync()
        {
            return await _dbSet
                .Include(x => x.User)
                .ToListAsync();
        }
        public override async Task<BuyerProfile?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<BuyerProfile>> GetAllPagedAsync(int pageIndex, int pageSize)
        {
            return await _dbSet
                .Include(x => x.User)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public Task<BuyerProfile?> GetProfileByUserGuid(Guid userId)
        {
            return _dbSet
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public Task<int> GetTotalProfileCount()
        {
            return _dbSet.CountAsync();
        }
    }
}
