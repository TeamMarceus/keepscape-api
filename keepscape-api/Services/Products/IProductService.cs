using keepscape_api.Dtos.Products;
using keepscape_api.QueryModels;

namespace keepscape_api.Services.Products
{
    public interface IProductService
    {
        Task<IEnumerable<ProductCategoryListDto>> GetProductCategories();
        Task<IEnumerable<ProductResponseDto>> Get(ProductQueryParameters productQueryParameters);
        Task<IEnumerable<ProductResponseDto>> GetALl();
    }
}
