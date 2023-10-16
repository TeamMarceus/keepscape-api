using AutoMapper;
using keepscape_api.Dtos.Products;
using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Interfaces;
using keepscape_api.Services.BaseImages;
using Microsoft.IdentityModel.Tokens;

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

            if (products.IsNullOrEmpty())
            {
                return new List<ProductResponseHomeDto>();
            }

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

        public async Task<ProductResponseDto?> GetById(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                return null;
            }

            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<bool> Update(Guid sellerId, Guid productId, ProductUpdateDto productUpdateDto)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                return false;
            }

            if (product.SellerProfile!.User!.Id != sellerId)
            {
                return false;
            }

            product = await UpdateProduct(product, productUpdateDto);

            return await _productRepository.UpdateAsync(product);
        }

        private async Task<Product> UpdateProduct(Product product, ProductUpdateDto productUpdateDto)
        {
            if (!string.IsNullOrEmpty(productUpdateDto.Name))
            {
                product.Name = productUpdateDto.Name;
            }
            if (!string.IsNullOrEmpty(productUpdateDto.Description))
            {
                product.Description = productUpdateDto.Description;
            }
            if (productUpdateDto.Price != null)
            {
                product.Price = productUpdateDto.Price.Value;
            }
            if (productUpdateDto.Quantity != null)
            {
                product.Quantity = productUpdateDto.Quantity.Value;
            }
            if (productUpdateDto.IsCustomizable != null)
            {
                product.IsCustomizable = productUpdateDto.IsCustomizable.Value;
            }
            if (productUpdateDto.IsHidden != null)
            {
                product.IsHidden = productUpdateDto.IsHidden.Value;
            }
            if (productUpdateDto.Images != null)
            {
                product.Images.Clear();

                foreach (var image in productUpdateDto.Images)
                {
                    var baseImage = await _baseImageService.Upload("products", image);

                    if (baseImage == null)
                    {
                        continue;
                    }

                    product.Images.Add(baseImage);
                }
            }
            if (productUpdateDto.CategoryIds != null)
            {
                product.Categories.Clear();

                foreach (var categoryId in productUpdateDto.CategoryIds)
                {
                    var category = await _categoryRepository.GetByIdAsync(categoryId);

                    if (category == null)
                    {
                        continue;
                    }

                    product.Categories.Add(category);
                }
            }

            return product;
        }

        public async Task Delete(Guid sellerId, Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                return;
            }

            if (product.SellerProfile!.User!.Id != sellerId)
            {
                return;
            }

            await _productRepository.DeleteAsync(product);
        }

        public async Task<bool> CreateReview(Guid userId, Guid productId, ProductReviewCreateDto productReviewCreateDto)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                return false;
            }

            var buyer = await _userRepository.GetByIdAsync(userId);

            if (buyer == null)
            {
                return false;
            }

            var productReview = _mapper.Map<ProductReview>(productReviewCreateDto);

            productReview.BuyerProfileId = buyer.BuyerProfile!.Id;
            productReview.ProductId = product.Id;

            return await _productRepository.AddProductReview(productReview);
        }
    }
}
