using keepscape_api.Dtos.Products;
using keepscape_api.QueryModels;

namespace keepscape_api.Services.Products
{
    public interface IProductService
    {
        Task<ProductResponseDto?> Create(Guid sellerId, ProductCreateDto productCreateDto);
        Task<bool> CreateReview(Guid userId, Guid productId, ProductReviewCreateDto productReviewCreateDto);
        //Task<bool> UpdateView(Guid userId, Guid productId, ProductReviewCreateDto productReviewCreateDto);
        Task<ProductResponseDto?> GetById(Guid productId);
        Task<IEnumerable<ProductCategoryPlaceDto>> GetPlaceCategories();
        Task<IEnumerable<ProductCategoryPlaceDto>> GetProductCategories();
        Task<IEnumerable<ProductResponseHomeDto>> Get(ProductQueryParameters productQueryParameters);
        Task<IEnumerable<ProductResponseDto>> GetAll();
        Task<bool> Update(Guid sellerId, Guid productId, ProductUpdateDto productUpdateDto);
        Task Delete(Guid sellerId, Guid productId);
    }
}
