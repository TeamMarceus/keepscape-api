using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface IProductReviewRepository : IBaseRepository<ProductReview>
    {
        Task<(IEnumerable<ProductReview> ProductReviews, int PageCount)> GetReviewsByProductId(Guid productId, ProductReviewQuery productReviewQuery);
        Task<ProductReview?> GetReviewByProductIdAndBuyerProfileId(Guid productId, Guid buyerProfileId);
    }
}
