using keepscape_api.Dtos.Products;
using keepscape_api.QueryModels;

namespace keepscape_api.Services.Products
{
    public interface IProductService
    {
        Task<ProductResponseDto?> Create(Guid sellerId, ProductCreateDto productCreateDto);
        Task<IEnumerable<ProductCategoryPlaceDto>> GetPlaceCategories();
        Task<IEnumerable<ProductCategoryPlaceDto>> GetProductCategories();
        Task<IEnumerable<ProductResponseHomeDto>> Get(ProductQueryParameters productQueryParameters);
        Task<IEnumerable<ProductResponseDto>> GetAll();
    }
}
