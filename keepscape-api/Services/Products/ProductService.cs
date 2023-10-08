using AutoMapper;
using keepscape_api.Dtos.Products;
using keepscape_api.Enums;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Interfaces;

namespace keepscape_api.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository productRepository, 
            ICategoryRepository categoryRepository, 
            IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductCategoryListDto>> GetProductCategories()
        {
            var categories = await _categoryRepository.GetAllAsync();

            var provincesCategories = categories.Where(c => c.Type == CategoryType.Provinces);
            var categoriesCategories = categories.Where(c => c.Type == CategoryType.Categories);

            var provinceProductCategories = new ProductCategoryListDto
            {
                Category = CategoryType.Provinces.ToString(),
                Subcategories = provincesCategories.Select(c => _mapper.Map<ProductCategoryDto>(c)).ToList()
            };
            var categoryProductCategories = new ProductCategoryListDto
            {
                Category = CategoryType.Categories.ToString(),
                Subcategories = categoriesCategories.Select(c => _mapper.Map<ProductCategoryDto>(c)).ToList()
            };

            return new List<ProductCategoryListDto>
            {
                provinceProductCategories,
                categoryProductCategories
            };
        }

        public Task<IEnumerable<ProductResponseDto>> Get(ProductQueryParameters productQueryParameters)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProductResponseDto>> GetALl()
        {
            return _mapper.Map<IEnumerable<ProductResponseDto>>(await _productRepository.GetAllAsync());
        }
    }
}
