using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class OrderReportRepository : BaseRepository<OrderReport>, IOrderReportRepository
    {
        public OrderReportRepository(APIDbContext context) : base(context)
        {
        }

        public async Task<OrderReport?> GetOrderReport(Guid orderId)
        {
            return await _dbSet
                .Include(x => x.Order)
                    .ThenInclude(x => x.Items)
                .Include(x => x.User)
                    .ThenInclude(x => x.BuyerProfile)
                .Where(o => !o.IsResolved)
                .AsSplitQuery()
                .SingleOrDefaultAsync(x => x.OrderId == orderId);
        }

        public override async Task<IEnumerable<OrderReport>> GetAllAsync()
        {
            return await _dbSet
                .Include(x => x.Order)
                    .ThenInclude(x => x.Items)
                .Include(x => x.User)
                    .ThenInclude(x => x.BuyerProfile)
                .Where(o => !o.IsResolved)
                .AsSplitQuery()
                .ToListAsync();
        }

        public override async Task<OrderReport?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(x => x.Order)
                    .ThenInclude(x => x.Items)
                .Include(x => x.User)
                    .ThenInclude(x => x.BuyerProfile)
                .Where(o => !o.IsResolved)
                .AsSplitQuery()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}
