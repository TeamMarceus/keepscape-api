using keepscape_api.Data;
using keepscape_api.Enums;
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

        public async Task<bool> AddProductReview(ProductReview productReview)
        {
            var product = await _dbSet.FindAsync(productReview.ProductId);

            if (product == null)
            {
                return false;
            }

            var productReviewExists = await _context.ProductReviews.AsNoTracking()
                .AnyAsync(pr => pr.ProductId == productReview.ProductId &&
                               pr.BuyerProfileId == productReview.BuyerProfileId);

            if (productReviewExists)
            {
                return false;
            }

            _context.ProductReviews.Add(productReview);

            await _context.SaveChangesAsync(); 

            var productReviews = await _context.ProductReviews.AsNoTracking()
                .Where(pr => pr.ProductId == productReview.ProductId)
                .ToListAsync();

            var averageRating = productReviews.Average(pr => pr.Rating);

            var freshProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            if (freshProduct != null)
            {
                freshProduct.Rating = (decimal)averageRating;
                await _context.SaveChangesAsync();
            }

            return true;
        }


        public async Task<IEnumerable<Product>> Get(ProductQueryParameters productQueryParameters)
        {
            var query = _dbSet
                .Include(p => p.Place)
                .Include(p => p.Images)
                .Include(p => p.SellerProfile)
                .Include(p => p.Categories)
                .Where(p => p.IsHidden == false && 
                p.Quantity > 0 && 
                p.SellerProfile!.SellerApplication!.Status == ApplicationStatus.Approved)
                .AsSplitQuery()
                .AsNoTracking();
            
            if (productQueryParameters.SellerId != null)
            {
                query = query.Where(p => p.SellerProfileId == productQueryParameters.SellerId);
            }
            if (!string.IsNullOrEmpty(productQueryParameters.Category))
            {
                query = query.Where(p => p.Categories
                .Any(pc => pc.Name == productQueryParameters.Category));
            }
            if (!string.IsNullOrEmpty(productQueryParameters.Province))
            {
                query = query.Where(p => p.Place!.Name
                == productQueryParameters.Province);
            }
            if (!string.IsNullOrEmpty(productQueryParameters.Search))
            {
                query = query.Where(p => p.Name.Contains(productQueryParameters.Search));
            }
            if (productQueryParameters.Descending)
            {
                query = query.OrderByDescending(p => p.DateTimeCreated);
            }
            else
            {
                query = query.OrderBy(p => p.DateTimeCreated);
            }
            if (productQueryParameters.MinPrice != null)
            {
                query = query.Where(p => p.Price >= productQueryParameters.MinPrice);
            }
            if (productQueryParameters.MaxPrice != null)
            {
                query = query.Where(p => p.Price <= productQueryParameters.MaxPrice);
            }
            if (productQueryParameters.Page != null && productQueryParameters.PageSize != null)
            {
                int skipAmount = ((int)productQueryParameters.Page - 1) * (int)productQueryParameters.PageSize;
                query = query.Skip(skipAmount).Take((int)productQueryParameters.PageSize);
            }

            return await query.ToListAsync();
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
                    .ThenInclude(pr => pr.BuyerProfile)
                        .ThenInclude(bp => bp!.User)
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
                _context.Categories.Attach(category);
            }

            return await base.UpdateAsync(product);
        }
    }
}
