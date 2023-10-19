using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(APIDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> GetByBuyerProfileId(Guid buyerProfileId)
        {
            return await _dbSet
                .Include(o => o.BuyerProfile)
                .Include(o => o.SellerProfile)
                .Include(o => o.OrderReport)
                .Include(o => o.DeliveryLogs)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.BuyerProfileId == buyerProfileId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetBySellerProfileId(Guid sellerProfileId)
        {
            return await _dbSet
                .Include(o => o.BuyerProfile)
                .Include(o => o.SellerProfile)
                .Include(o => o.OrderReport)
                .Include(o => o.DeliveryLogs)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.SellerProfileId == sellerProfileId)
                .ToListAsync();
        }

        public override async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(o => o.BuyerProfile)
                .Include(o => o.SellerProfile)
                .Include(o => o.OrderReport)
                .Include(o => o.DeliveryLogs)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .SingleOrDefaultAsync(o => o.Id == id);
        }

        public override async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.BuyerProfile)
                .Include(o => o.SellerProfile)
                .Include(o => o.OrderReport)
                .Include(o => o.DeliveryLogs)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByProductId(Guid productId)
        {
            return await _dbSet
                .Include(o => o.Items)
                .Where(o => o.Items.Any(oi => oi.ProductId == productId))
                .ToListAsync();
        }
    }
}
