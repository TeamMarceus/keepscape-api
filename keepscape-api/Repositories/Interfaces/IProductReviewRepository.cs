using keepscape_api.Models;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface IProductReviewRepository : IBaseRepository<ProductReview>
    {
        Task<IEnumerable<ProductReview>> GetReviewsByProductId(Guid productId);
        Task<ProductReview?> GetReviewByProductIdAndBuyerProfileId(Guid productId, Guid buyerProfileId);
    }
}
