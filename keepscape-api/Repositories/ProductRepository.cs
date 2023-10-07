using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(APIDbContext context) : base(context) 
        {
        }

        public async Task<IEnumerable<Product>> Get(ProductQueryParameters productQueryParameters)
        {
            var query =  _dbSet
                .Include(p => p.ProductImages)
                .ThenInclude(pi => pi.BaseImage)
                .Include(p => p.SellerProfile)
                .Include(p => p.ProductReviews)
                    .ThenInclude(pr => pr.BuyerProfile)
                        .ThenInclude(bp => bp!.User)
                .AsSplitQuery()
                .AsQueryable();
            
            if (productQueryParameters.SellerId != null)
            {
                query = query.Where(p => p.SellerProfileId == productQueryParameters.SellerId);
            }
            if (productQueryParameters.Category != null)
            {
                query = query.Where(p => p.ProductCategories.Any(pc => pc.Name == productQueryParameters.Category));
            }
            if (productQueryParameters.Province != null)
            {
                query = query.Where(p => p.PlaceCategory != null && p.PlaceCategory.Name == productQueryParameters.Province);
            }
            if (productQueryParameters.Search != null)
            {
                query = query.Where(p => p.Name.Contains(productQueryParameters.Search));
            }
            if (productQueryParameters.Page != null && productQueryParameters.PageSize != null)
            {
                query = query.Skip(((int)productQueryParameters.Page - 1) * (int)productQueryParameters.PageSize);
            }
            if (productQueryParameters.MinPrice != null)
            {
                   query = query.Where(p => p.Price >= productQueryParameters.MinPrice);
            }
            if (productQueryParameters.MaxPrice != null)
            {
                   query = query.Where(p => p.Price <= productQueryParameters.MaxPrice);
            }
            if (productQueryParameters.Descending)
            {
                query = query.OrderByDescending(p => p.DateTimeCreated);
            }
            else
            {
                query = query.OrderBy(p => p.DateTimeCreated);
            }

            return await query.ToListAsync();
        }

        public override async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbSet
                .Include(p => p.ProductImages)
                    .ThenInclude(pi => pi.BaseImage)
                .Include(p => p.PlaceCategory)
                .Include(p => p.SellerProfile)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Name)
                .OrderByDescending(p => p.DateTimeCreated)  
                .ToListAsync();
        }
        public override async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.SellerProfile)
                .Include(p => p.ProductImages)
                .ThenInclude(pi => pi.BaseImage)
                .Include(p => p.ProductReviews)
                    .ThenInclude(pr => pr.BuyerProfile)
                        .ThenInclude(bp => bp!.User)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<int> GetTotalProductCount()
        {
            return _dbSet.CountAsync();
        }
    }
}
