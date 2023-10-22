using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class ProductReviewRepository : BaseRepository<ProductReview>, IProductReviewRepository
    {
        public ProductReviewRepository(APIDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductReview>> GetReviewsByProductId(Guid productId)
        {
            return await _dbSet
                .Include(pr => pr.BuyerProfile)
                .Where(pr => pr.ProductId == productId)
                .ToListAsync();
        }

        public override Task<ProductReview?> GetByIdAsync(Guid id)
        {
            return base.GetByIdAsync(id);
        }

        public override Task<IEnumerable<ProductReview>> GetAllAsync()
        {
            return base.GetAllAsync();
        }

        public new async Task<ProductReview> AddAsync(ProductReview productReview)
        {
            _context.Products.Attach(productReview.Product!);
            _context.BuyerProfiles.Attach(productReview.BuyerProfile!);

            _dbSet.Add(productReview);

            await _context.SaveChangesAsync();

            var productReviews = await _dbSet
                .Where(pr => pr.ProductId == productReview.ProductId)
                .ToListAsync();

            var averageProductRating = productReviews.Average(pr => pr.Rating);

            var freshProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == productReview.Product!.Id);
            if (freshProduct != null)
            {
                freshProduct.Rating = (decimal)averageProductRating;
                await _context.SaveChangesAsync();
            }

            var sellerProducts = await _context.Products.Where(p => p.SellerProfileId == productReview.Product!.SellerProfileId).ToListAsync();
            var averageSellerRating = sellerProducts.Average(p => p.Rating);

            var freshSellerProfile = await _context.SellerProfiles.FirstOrDefaultAsync(sp => sp.Id == productReview.Product!.SellerProfileId);
            if (freshSellerProfile != null)
            {
                freshSellerProfile.Rating = averageSellerRating;
            }

            await _context.SaveChangesAsync();

            return productReview;
        }

        public async Task<ProductReview?> GetReviewByProductIdAndBuyerProfileId(Guid productId, Guid buyerProfileId)
        {
            return await _dbSet
                .Include(pr => pr.BuyerProfile)
                .FirstOrDefaultAsync(pr => pr.ProductId == productId && pr.BuyerProfileId == buyerProfileId);
        }

        public new async Task<bool> UpdateAsync(ProductReview productReview)
        {
            await _context.SaveChangesAsync();

            var productReviews = await _dbSet
                .Where(pr => pr.ProductId == productReview.ProductId)
                .ToListAsync();

            var averageRating = productReviews.Average(pr => pr.Rating);

            var freshProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == productReview.Product!.Id);
            if (freshProduct != null)
            {
                freshProduct.Rating = (decimal)averageRating;
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public new async Task<bool> DeleteAsync(ProductReview productReview)
        {
            await base.DeleteAsync(productReview);

            var productReviews = await _dbSet
                .Where(pr => pr.ProductId == productReview.ProductId)
                .ToListAsync();

            var averageRating = productReviews.Average(pr => pr.Rating);

            var freshProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == productReview.Product!.Id);
            if (freshProduct != null)
            {
                freshProduct.Rating = (decimal)averageRating;
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<(IEnumerable<ProductReview> ProductReviews, int PageCount)> GetReviewsByProductId(Guid productId, ProductReviewQuery productReviewQuery)
        {
            var query = _dbSet
                .Include(pr => pr.BuyerProfile)
                .Where(pr => pr.ProductId == productId)
                .AsQueryable();

            if (productReviewQuery.Stars != null)
            {
                query = query.Where(pr => pr.Rating == productReviewQuery.Stars);
            }

            int pageCount = 1;

            if (query.Count() == 0)
            {
                return (new List<ProductReview>(), 0);
            }

            if (productReviewQuery.Page != null && productReviewQuery.PageSize != null)
            {
                int queryPageCount = await query.CountAsync();

                pageCount = (int)Math.Ceiling((double)queryPageCount / (int)productReviewQuery.PageSize);

                if (productReviewQuery.Page > pageCount)
                {
                    productReviewQuery.Page = pageCount;
                }
                else if (productReviewQuery.Page < 1)
                {
                    productReviewQuery.Page = 1;
                }
                else if (pageCount == 0)
                {
                    pageCount = 1;
                    productReviewQuery.Page = 1;
                }

                int skipAmount = ((int)productReviewQuery.Page - 1) * (int)productReviewQuery.PageSize;
                query = query.Skip(skipAmount).Take((int)productReviewQuery.PageSize);
            }

            return (await query.ToListAsync(), pageCount);
        }
    }
}
