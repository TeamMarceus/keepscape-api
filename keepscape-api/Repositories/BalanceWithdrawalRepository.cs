using keepscape_api.Data;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.QueryModels;
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
                    .ThenInclude(b => b.User)
                        .ThenInclude(u => u!.SellerProfile)
                .Where(x => x.Status == PaymentStatus.Pending)
                .ToListAsync();
        }

        public override async Task<BalanceWithdrawal?> GetByIdAsync(Guid Id)
        {
            return await _dbSet
                .Include(x => x.Balance)
                .ThenInclude(b => b.User)
                        .ThenInclude(u => u!.SellerProfile)
                .Where(x => x.Status == PaymentStatus.Pending)
                .FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<(IEnumerable<BalanceWithdrawal> BalanceWithdrawals, int PageCount)> GetByBalanceId(Guid balanceId, PaginatorQuery paginatorQuery)
        {
            var query = _dbSet
                .Include(x => x.Balance)
                .ThenInclude(b => b.User)
                        .ThenInclude(u => u!.SellerProfile)
                .Where(x => x.BalanceId == balanceId)
                .OrderByDescending(x => x.DateTimeCreated)
                .AsQueryable();

            int pageCount = 1;

            if (query.Count() == 0)
            {
                return (await query.ToListAsync(), 0);
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

        public async Task<(IEnumerable<BalanceWithdrawal> BalanceWithdrawals, int PageCount)> Get(BalanceWithdrawalQuery balanceWithdrawalQuery)
        {
            var query = _dbSet
                .Include(x => x.Balance)
                .ThenInclude(b => b.User)
                        .ThenInclude(u => u!.SellerProfile)
                .OrderByDescending(x => x.DateTimeCreated)
                .AsQueryable();

            int pageCount = 1;
            if (!string.IsNullOrEmpty(balanceWithdrawalQuery.PaymentStatus))
            {
                var statusValid = Enum.TryParse<PaymentStatus>(balanceWithdrawalQuery.PaymentStatus, out var status);

                if (statusValid)
                {
                    query = query.Where(x => x.Status == status);
                }
            }
            if (!string.IsNullOrEmpty(balanceWithdrawalQuery.PaymentMethod))
            {
                var paymentMethodValid = Enum.TryParse<PaymentMethod>(balanceWithdrawalQuery.PaymentMethod, out var paymentMethod);

                if (paymentMethodValid)
                {
                    query = query.Where(x => x.PaymentMethod == paymentMethod);
                }
            }
            if (!string.IsNullOrEmpty(balanceWithdrawalQuery.SellerName))
            {
                query = query.Where(x => x.Balance.User!.SellerProfile!.Name.Contains(balanceWithdrawalQuery.SellerName));
            }
            if (query.Count() == 0)
            {
                return (await query.ToListAsync(), 0);
            }
            if (balanceWithdrawalQuery.Page != null && balanceWithdrawalQuery.PageSize != null)
            {
                int queryPageCount = await query.CountAsync();

                pageCount = (int)Math.Ceiling((double)queryPageCount / (int)balanceWithdrawalQuery.PageSize);

                if (balanceWithdrawalQuery.Page > pageCount)
                {
                    balanceWithdrawalQuery.Page = pageCount;
                }
                else if (balanceWithdrawalQuery.Page < 1)
                {
                    balanceWithdrawalQuery.Page = 1;
                }
                else if (pageCount == 0)
                {
                    pageCount = 1;
                    balanceWithdrawalQuery.Page = 1;
                }

                int skipAmount = ((int)balanceWithdrawalQuery.Page - 1) * (int)balanceWithdrawalQuery.PageSize;
                query = query.Skip(skipAmount).Take((int)balanceWithdrawalQuery.PageSize);
            }

            return (await query.ToListAsync(), pageCount);
        }
    }
}
