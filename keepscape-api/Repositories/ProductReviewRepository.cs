using keepscape_api.Data;
using keepscape_api.Models;
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

            _context.ProductReviews.Add(productReview);

            await _context.SaveChangesAsync();

            var productReviews = await _context.ProductReviews
                .Where(pr => pr.ProductId == productReview.ProductId)
                .ToListAsync();

            var averageRating = productReviews.Average(pr => pr.Rating);

            var freshProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == productReview.Product!.Id);
            if (freshProduct != null)
            {
                freshProduct.Rating = (decimal)averageRating;
                await _context.SaveChangesAsync();
            }

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

            var productReviews = await _context.ProductReviews
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

            var productReviews = await _context.ProductReviews
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
    }
}
