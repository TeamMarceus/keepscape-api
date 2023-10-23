using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class ProductReportRepository : BaseRepository<ProductReport>, IProductReportRepository
    {
        public ProductReportRepository(APIDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<ProductReport> ProductReports, int PageCount)> Get(ProductReportQuery productReportQuery)
        {
            var query = _dbSet
                .Include(x => x.User)
                .Include(x => x.Product)
                    .ThenInclude(x => x.SellerProfile)
                    .ThenInclude(x => x!.User)
                .Where(x => !x.IsResolved)
                .AsQueryable();

            int pageCount = 1;

            if (!string.IsNullOrEmpty(productReportQuery.SellerName))
            {
                query = query.Where(x => x.Product.SellerProfile!.Name.ToLower().Contains(productReportQuery.SellerName.ToLower()));
            }
            if (!string.IsNullOrEmpty(productReportQuery.ProductName))
            {
                query = query.Where(x => x.Product!.Name.ToLower().Contains(productReportQuery.ProductName.ToLower()));
            }
            if (query.Count() == 0)
            {
                return (await query.ToListAsync(), 0);
            }
            if (productReportQuery.Page != null && productReportQuery.PageSize != null)
            {
                int queryPageCount = await query.CountAsync();

                pageCount = (int)Math.Ceiling((double)queryPageCount / (int)productReportQuery.PageSize);

                if (productReportQuery.Page > pageCount)
                {
                    productReportQuery.Page = pageCount;
                }
                else if (productReportQuery.Page < 1)
                {
                    productReportQuery.Page = 1;
                }
                else if (pageCount == 0)
                {
                    pageCount = 1;
                    productReportQuery.Page = 1;
                }

                int skipAmount = ((int)productReportQuery.Page - 1) * (int)productReportQuery.PageSize;
                query = query.Skip(skipAmount).Take((int)productReportQuery.PageSize);
            }

            return (await query.ToListAsync(), pageCount);
        }

        public override async Task<IEnumerable<ProductReport>> GetAllAsync()
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.Product)
                    .ThenInclude(x => x.SellerProfile)
                    .ThenInclude(x => x!.User)
                .Where(x => !x.IsResolved)
                .ToListAsync();
        }
        public override async Task<ProductReport?> GetByIdAsync(Guid Id)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.Product)
                    .ThenInclude(x => x.SellerProfile)
                    .ThenInclude(x => x!.User)
                .Where(x => !x.IsResolved)
                .FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<IEnumerable<ProductReport>> GetByProductIdAsync(Guid productId)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.Product)
                    .ThenInclude(x => x.SellerProfile)
                    .ThenInclude(x => x!.User)
                .Where(x => x.ProductId == productId && !x.IsResolved)
                .ToListAsync();
        }

        public async Task<bool> UpdateRangeAsync(IEnumerable<ProductReport> productReports)
        {
            _dbSet.UpdateRange(productReports);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
