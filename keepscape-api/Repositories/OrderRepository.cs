using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.Repositories.Generics;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(APIDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _dbSet
                .Include(t => t.OrderDeliveryLogs)
                .Include(t => t.Product)
                    .ThenInclude(t => t!.SellerProfile)
                .ToListAsync();
        }
        public override async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(t => t.OrderDeliveryLogs)
                .Include(t => t.Product)
                    .ThenInclude(t => t!.SellerProfile)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task<Order?> CreateDeliveryLogByOrderId(Guid orderId, OrderDeliveryLog orderDeliveryLog)
        {
            var order = await _dbSet.Where(t => t.Id == orderId).FirstOrDefaultAsync();

            if (order == null)
            {
                return null;
            }

            order.OrderDeliveryLogs.Add(orderDeliveryLog);

            return await _context.SaveChangesAsync() > 0 ? order : null;
        }
        public async Task<IEnumerable<Order>> GetOrdersByBuyerProfileId(Guid buyerProfileId)
        {
            return await _dbSet
                .Where(t => t.BuyerProfileId == buyerProfileId)
                .Include(t => t.OrderDeliveryLogs)
                .Include(t => t.Product)
                    .ThenInclude(t => t!.SellerProfile)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersBySellerProfileId(Guid sellerProfileId)
        {
            return await _dbSet
                .Include(t => t.Product)
                    .ThenInclude(p => p.SellerProfile)
                .Include(t => t.OrderDeliveryLogs)
                .Where(t => t.Product != null && t.Product.SellerProfileId == sellerProfileId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllPagedAsync(int pageIndex, int pageSize)
        {
            return await _dbSet
                .Include(t => t.OrderDeliveryLogs)
                .Include(t => t.Product)
                    .ThenInclude(t => t!.SellerProfile)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
