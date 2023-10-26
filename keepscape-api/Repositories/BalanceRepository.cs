using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.QueryModels;
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
                .Include(b => b.User)
                .Include(b => b.Histories)
                .Include(b => b.Withdrawals)
                .ToListAsync();
        }
        public override async Task<Balance?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(b => b.User)
                .Include(b => b.Histories)
                .Include(b => b.Withdrawals)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        public async Task<Balance?> GetByUserId(Guid userId)
        {
            return await _dbSet
                .Include(b => b.User)
                .Include(b => b.Histories)
                .Include(b => b.Withdrawals)
                .FirstOrDefaultAsync(b => b.UserId == userId);
        }

        public async Task<(IEnumerable<BalanceLog> BalanceLogs, int PageCount)> GetLogsByBalanceId(Guid balanceId, PaginatorQuery paginatorQuery)
        {
            var query = _context.BalanceLogs
                .Where(b => b.BalanceId == balanceId)
                .OrderByDescending(b => b.DateTimeCreated)
                .AsQueryable();

            int pageCount = 1;

            if (query.Count() == 0)
            {
                return (await query.ToListAsync(), pageCount);
            }

            if (paginatorQuery.Page != null && paginatorQuery.PageSize != null)
            {
                int queryPageCount = await query.CountAsync();

                pageCount = (int)Math.Ceiling((double)queryPageCount / (int)paginatorQuery.PageSize);

                if (paginatorQuery.Page > pageCount)
                {
                    paginatorQuery.Page = pageCount;
                }
                else if (paginatorQuery.Page < 1)
                {
                    paginatorQuery.Page = 1;
                }
                else if (pageCount == 0)
                {
                    pageCount = 1;
                    paginatorQuery.Page = 1;
                }

                int skipAmount = ((int)paginatorQuery.Page - 1) * (int)paginatorQuery.PageSize;
                query = query.Skip(skipAmount).Take((int)paginatorQuery.PageSize);
            }

            return (await query.ToListAsync(), pageCount);
        }
    }
}
