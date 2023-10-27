using keepscape_api.Data;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace keepscape_api.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(APIDbContext context) : base(context) 
        {
        }

        public async Task<(IEnumerable<Product> Products, int PageCount)> Get(ProductQuery productQueryParameters)
        {
            var query = _dbSet
                .Include(p => p.Place)
                .Include(p => p.Images)
                .Include(p => p.SellerProfile)
                    .ThenInclude(sp => sp!.User)
                .Include(p => p.Categories)
                .Include(p => p.Reviews)
                .Where(
                    p => p.Quantity > 0 && 
                    p.SellerProfile!.SellerApplication!.Status == ApplicationStatus.Approved &&
                    p.DateTimeDeleted == null &&
                    !p.SellerProfile.User!.IsBanned
                )
                .AsSplitQuery()
                .AsNoTracking();


            int pageCount = 1;

            if (productQueryParameters.SellerProfileId != null)
            {
                query = query.Where(p => p.SellerProfileId == productQueryParameters.SellerProfileId);
            }
            if (productQueryParameters.IsHidden)
            {
                query = query.Where(p => p.IsHidden == productQueryParameters.IsHidden);
            }
            if (!productQueryParameters.Categories.IsNullOrEmpty())
            {
                query = query.Where(p => p.Categories.Any(c => productQueryParameters.Categories.Contains(c.Name)));
            }
            if (!productQueryParameters.Provinces.IsNullOrEmpty())
            {
                query = query.Where(p => p.Place != null && productQueryParameters.Provinces.Contains(p.Place.Name));
            }
            if (!string.IsNullOrEmpty(productQueryParameters.Search))
            {
                query = query.Where(
                    p => p.Name.Contains(productQueryParameters.Search) ||
                    p.Description.Contains(productQueryParameters.Search) ||
                    p.Place!.Name.Contains(productQueryParameters.Search) ||
                    p.Categories.Any(x => x.Name.Contains(productQueryParameters.Search)) || 
                    p.SellerProfile!.Name.Contains(productQueryParameters.Search)
                );
            }
            if (productQueryParameters.Descending)
            {
                query = query.OrderByDescending(p => p.DateTimeCreated);
            }
            else
            {
                query = query.OrderBy(p => p.DateTimeCreated);
            }
            if(productQueryParameters.Rating != null)
            {
                query = query.Where(p => p.Rating >= (int)productQueryParameters.Rating);
            }
            if (productQueryParameters.MinPrice != null)
            {
                query = query.Where(p => p.BuyerPrice >= productQueryParameters.MinPrice);
            }
            if (productQueryParameters.MaxPrice != null)
            {
                query = query.Where(p => p.BuyerPrice <= productQueryParameters.MaxPrice);
            }
            if (query.Count() == 0)
            {
                return (new List<Product>(), 0);
            }
            if (productQueryParameters.Page != null && productQueryParameters.PageSize != null)
            {
                int queryPageCount = await query.CountAsync();
                
                pageCount = (int)Math.Ceiling((double)queryPageCount / (int)productQueryParameters.PageSize);

                if (productQueryParameters.Page > pageCount)
                {
                    productQueryParameters.Page = pageCount;
                }
                else if (productQueryParameters.Page < 1)
                {
                    productQueryParameters.Page = 1;
                }
                else if (pageCount == 0)
                {
                    pageCount = 1;
                    productQueryParameters.Page = 1;
                }

                int skipAmount = ((int)productQueryParameters.Page - 1) * (int)productQueryParameters.PageSize;
                query = query.Skip(skipAmount).Take((int)productQueryParameters.PageSize);
            }

            return (await query.ToListAsync(), pageCount);
        }

        public override async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbSet
                .Include(p => p.Images)
                .Include(p => p.Categories)
                .Include(p => p.Place)
                .Include(p => p.SellerProfile)
                .OrderByDescending(p => p.DateTimeCreated)  
                .ToListAsync();
        }
        public override async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.SellerProfile)
                .Include(p => p.Images)
                .Include(p => p.Place)
                .Include(p => p.Categories)
                .Include(p => p.Reviews)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<int> GetTotalProductCount()
        {
            return _dbSet.CountAsync();
        }

        public new async Task<Product> AddAsync(Product product)
        {
            foreach (var category in product.Categories)
            {
                _context.Categories.Attach(category);
            }

            _context.SellerProfiles.Attach(product.SellerProfile!);
            _context.Places.Attach(product.Place!);

            return await base.AddAsync(product);
        }

        public new async Task<bool> UpdateAsync(Product product)
        {
            foreach (var category in product.Categories)
            {
                if (_context.Entry(category).State == EntityState.Detached)
                {
                    _context.Categories.Attach(category);
                }
            }

            return await base.UpdateAsync(product);
        }
    }
}
