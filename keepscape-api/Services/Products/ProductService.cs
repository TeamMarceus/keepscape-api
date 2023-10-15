using AutoMapper;
using keepscape_api.Dtos.Products;
using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Interfaces;
using keepscape_api.Services.BaseImages;

namespace keepscape_api.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPlaceRepository _placeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBaseImageService _baseImageService;
        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository productRepository, 
            ICategoryRepository categoryRepository, 
            IPlaceRepository placeRepository,
            IUserRepository userRepository,
            IBaseImageService baseImageService,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _placeRepository = placeRepository;
            _baseImageService = baseImageService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductCategoryPlaceDto>> GetPlaceCategories()
        {
            var provincesCategories = await _placeRepository.GetAllAsync();


            return provincesCategories.Select(c => _mapper.Map<ProductCategoryPlaceDto>(c)).ToList();
        }

        public async Task<IEnumerable<ProductCategoryPlaceDto>> GetProductCategories()
        {
            var productCategories = await _categoryRepository.GetAllAsync();


            return productCategories.Select(c => _mapper.Map<ProductCategoryPlaceDto>(c)).ToList();
        }

        public async Task<IEnumerable<ProductResponseHomeDto>> Get(ProductQueryParameters productQueryParameters)
        {
            var products = await _productRepository.Get(productQueryParameters);

            return products.Select(p => _mapper.Map<ProductResponseHomeDto>(p));
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAll()
        {
            return _mapper.Map<IEnumerable<ProductResponseDto>>(await _productRepository.GetAllAsync());
        }

        public async Task<ProductResponseDto?> Create(Guid sellerId, ProductCreateDto productCreateDto)
        {
            var seller = await _userRepository.GetByIdAsync(sellerId);

            if (seller == null)
            {
                return null;
            }

            var product = _mapper.Map<Product>(productCreateDto);

            product.SellerProfile = seller.SellerProfile;

            foreach (var image in productCreateDto.ProductImages)
            {
                var baseImage = await _baseImageService.Upload("products", image);

                if (baseImage == null)
                {
                    continue;
                }

                product.Images.Add(baseImage);
            }

            var createdProduct = await _productRepository.AddAsync(product);

            return _mapper.Map<ProductResponseDto>(createdProduct);
        }
    }
}
