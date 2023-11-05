using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(APIDbContext context) : base(context)
        {
        }

        public override async Task<OrderItem?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(x => x.Order)
                .Include(x => x.Product)
                .Include(x => x.Gift)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public override async Task<IEnumerable<OrderItem>> GetAllAsync()
        {
            return await _dbSet
                .Include(x => x.Order)
                .Include(x => x.Product)
                .Include(x => x.Gift)
                .ToListAsync();
        }
    }
}
