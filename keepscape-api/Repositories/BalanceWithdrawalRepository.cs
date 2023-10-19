using keepscape_api.Data;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class BalanceWithdrawalRepository : BaseRepository<BalanceWithdrawal>, IBalanceWithdrawalRepository
    {
        public BalanceWithdrawalRepository(APIDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<BalanceWithdrawal>> GetAllAsync()
        {
            return await _dbSet
                .Include(x => x.Balance)
                .Where(x => x.Status == PaymentStatus.Pending)
                .ToListAsync();
        }

        public override async Task<BalanceWithdrawal?> GetByIdAsync(Guid Id)
        {
            return await _dbSet
                .Include(x => x.Balance)
                .Where(x => x.Status == PaymentStatus.Pending)
                .FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<BalanceWithdrawal?> GetByBalanceId(Guid balanceId)
        {
            return await _dbSet
                .Include(x => x.Balance)
                .Where(x => x.Status == PaymentStatus.Pending)
                .FirstOrDefaultAsync(x => x.BalanceId == balanceId);
        }
    }
}
