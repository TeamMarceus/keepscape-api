using keepscape_api.Data;
using keepscape_api.Models.Checkouts.Products;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class ProductReportRepository : BaseRepository<ProductReport>, IProductReportRepository
    {
        public ProductReportRepository(APIDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<ProductReport>> GetAllAsync()
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.Product)
                    .ThenInclude(x => x.SellerProfile)
                .ToListAsync();
        }
        public override async Task<ProductReport?> GetByIdAsync(Guid Id)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.Product)
                    .ThenInclude(x => x.SellerProfile)
                .FirstOrDefaultAsync(x => x.Id == Id);
        }
    }
}
