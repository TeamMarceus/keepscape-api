using keepscape_api.Dtos.Products;
using keepscape_api.QueryModels;

namespace keepscape_api.Services.Products
{
    public interface IProductService
    {
        Task<ProductResponseDto?> Create(Guid sellerId, ProductCreateDto productCreateDto);
        Task<ProductCategoryPlaceDto?> CreatePlace(ProductCategoryPlaceCreateDto productCategoryPlaceCreateDto);
        Task<ProductCategoryPlaceDto?> CreateCategory(ProductCategoryPlaceCreateDto productCategoryPlaceCreateDto);
        Task<bool> CreateReview(Guid userId, Guid productId, ProductReviewCreateDto productReviewCreateDto);
        Task<ProductReviewResponseDto?> GetReview(Guid userId, Guid productId);
        Task<bool> UpdateReview(Guid userId, Guid productId, ProductReviewUpdateDto productReviewCreateDto);
        Task<bool> UpdatePlace(Guid placeId, IFormFile image);
        Task<bool> UpdateCategory(Guid categoryId, IFormFile image);
        Task<ProductResponseDto?> GetById(Guid productId);
        Task<ProductResponseHomePaginatedDto> Get(ProductQuery productQueryParameters);
        Task<ProductResponsePaginatedDto?> GetBySellerId(Guid userId, ProductQuery productQueryParameters);
        Task<ProductReviewPaginatedDto> GetReviews(Guid productId, ProductReviewQuery productReviewQuery);
        Task<ProductSellerDto?> GetSellerProfile(Guid sellerProfileId);
        Task<IEnumerable<ProductResponseDto>> GetAll();
        Task<IEnumerable<ProductCategoryPlaceDto>> GetPlaceCategories();
        Task<IEnumerable<ProductCategoryPlaceDto>> GetProductCategories();
        Task<bool> Update(Guid sellerId, Guid productId, ProductUpdateDto productUpdateDto);
        Task Delete(Guid sellerId, Guid productId);
        Task DeleteReview(Guid reviewId);
        Task DeletePlace(Guid placeId);
        Task DeleteCategory(Guid categoryId);
    }
}
