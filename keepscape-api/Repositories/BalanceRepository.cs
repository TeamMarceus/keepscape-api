using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class BalanceRepository : BaseRepository<Balance>, IBalanceRepository
    {
        public BalanceRepository(APIDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Balance>> GetAllAsync()
        {
            return await _dbSet
                .Include(b => b.BalanceHistories)
                .ToListAsync();
        }
        public override async Task<Balance?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(b => b.BalanceHistories)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        public async Task<Balance?> GetBalanceByUserId(Guid userId)
        {
            return await _dbSet
                .Include(b => b.BalanceHistories)
                .FirstOrDefaultAsync(b => b.UserId == userId);
        }
    }
}
