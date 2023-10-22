using keepscape_api.Data;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.QueryModels;
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

        public async Task<(IEnumerable<Order> Orders, int PageCount)> Get(OrderQuery orderQuery)
        {
            var query = _dbSet
                .Include(o => o.BuyerProfile)
                    .ThenInclude(b => b!.User)
                .Include(o => o.SellerProfile)
                    .ThenInclude(s => s!.User)
                .Include(o => o.OrderReport)
                .Include(o => o.DeliveryLogs)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .AsQueryable();

            int pageCount = 1;
            if (orderQuery.ProductName != null)
            {
                query = query.Where(o => o.Items.Any(oi => oi.Product!.Name == orderQuery.ProductName));
            }
            if (orderQuery.Status != null)
            {
                if (orderQuery.Status == "Completed")
                {
                    query = query.Where(o => o.Status == OrderStatus.Cancelled || o.Status == OrderStatus.Delivered);
                }
                else
                {
                    var orderStatus = Enum.TryParse<OrderStatus>(orderQuery.Status, out var status);

                    if (orderStatus)
                    {
                        query = query.Where(o => o.Status == status);
                    }
                }     
            }
            if (query.Count() == 0)
            {
                return (await query.ToListAsync(), 0);
            }
            if (orderQuery.Page != null && orderQuery.PageSize != null)
            {
                var skip = (int)orderQuery.Page * (int)orderQuery.PageSize;
                query = query.Skip(skip);
                query = query.Take((int)orderQuery.PageSize);
                pageCount = (int)Math.Ceiling((double)query.Count() / (int)orderQuery.PageSize);
            }

            return (await query.ToListAsync(), pageCount);
        }
    }
}
