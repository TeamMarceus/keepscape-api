using AutoMapper;
using keepscape_api.Dtos.Products;
using keepscape_api.Models;
using keepscape_api.Models.Categories;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Interfaces;
using keepscape_api.Services.BaseImages;
using Microsoft.IdentityModel.Tokens;

namespace keepscape_api.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPlaceRepository _placeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBaseImageService _baseImageService;
        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository productRepository, 
            IProductReviewRepository productReviewRepository,
            ICategoryRepository categoryRepository, 
            IPlaceRepository placeRepository,
            IUserRepository userRepository,
            IBaseImageService baseImageService,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _productReviewRepository = productReviewRepository;
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

        public async Task<IEnumerable<ProductResponseHomeDto>> Get(ProductQuery productQueryParameters)
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

            var productReviewExist = await _productReviewRepository.GetReviewByProductIdAndBuyerProfileId(productId, buyer.BuyerProfile!.Id);

            if (productReviewExist != null)
            {
                return false;
            }

            var productReview = _mapper.Map<ProductReview>(productReviewCreateDto);

            productReview.BuyerProfileId = buyer.BuyerProfile!.Id;
            productReview.ProductId = product.Id;

            await _productReviewRepository.AddAsync(productReview);

            return true;
        }

        public async Task<bool> UpdateReview(Guid userId, Guid productId, ProductReviewUpdateDto productReviewCreateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null) 
            { 
                return false;
            }

            var productReview = await _productReviewRepository.GetReviewByProductIdAndBuyerProfileId(productId, user.BuyerProfile!.Id);

            if (productReview == null)
            {
                return false;
            }

            productReview.Review = productReviewCreateDto.Review ?? productReview.Review;
            productReview.Rating = productReviewCreateDto.Rating ?? productReview.Rating;

            return await _productReviewRepository.UpdateAsync(productReview);
        }

        public async Task DeleteReview(Guid reviewId)
        {
            var productReview = await _productReviewRepository.GetByIdAsync(reviewId);
            
            if (productReview == null)
            {
                return;
            }

            await _productReviewRepository.DeleteAsync(productReview);
        }

        public async Task<ProductReviewResponseDto?> GetReview(Guid userId, Guid productId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var productReview = await _productReviewRepository.GetReviewByProductIdAndBuyerProfileId(productId, user.BuyerProfile!.Id);

            if (productReview == null)
            {
                return null;
            }

            return _mapper.Map<ProductReviewResponseDto>(productReview);
        }

        public async Task<ProductCategoryPlaceDto?> CreatePlace(ProductCategoryPlaceCreateDto productCategoryPlaceCreateDto)
        {
            var baseImage = await _baseImageService.Upload("places", productCategoryPlaceCreateDto.Image);

            if (baseImage == null)
            {
                return null;
            }

            var place = new Place
            {
                Name = productCategoryPlaceCreateDto.Name,
                BaseImage = baseImage
            };

            var createdPlace = await _placeRepository.AddAsync(place);

            return _mapper.Map<ProductCategoryPlaceDto>(createdPlace);
        }

        public async Task<ProductCategoryPlaceDto?> CreateCategory(ProductCategoryPlaceCreateDto productCategoryPlaceCreateDto)
        {
            var baseImage = await _baseImageService.Upload("categories", productCategoryPlaceCreateDto.Image);

            if (baseImage == null)
            {
                return null;
            }

            var category = new Category
            {
                Name = productCategoryPlaceCreateDto.Name,
                BaseImage = baseImage
            };

            var createdCategory = _categoryRepository.AddAsync(category);

            return _mapper.Map<ProductCategoryPlaceDto>(createdCategory);
        }

        public async Task<bool> UpdatePlace(Guid placeId, IFormFile image)
        {
            var place = await _placeRepository.GetByIdAsync(placeId);

            if (place == null)
            {
                return false;
            }

            var baseImage = await _baseImageService.Upload("places", image);

            if (baseImage == null)
            {
                return false;
            }

            place.BaseImage = baseImage;

            return await _placeRepository.UpdateAsync(place);
        }

        public async Task<bool> UpdateCategory(Guid categoryId, IFormFile image)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);

            if (category == null)
            {
                return false;
            }

            var baseImage = await _baseImageService.Upload("categories", image);

            if (baseImage == null)
            {
                return false;
            }

            category.BaseImage = baseImage;

            return await _categoryRepository.UpdateAsync(category);
        }

        public async Task DeletePlace(Guid placeId)
        {
            var place = await _placeRepository.GetByIdAsync(placeId);

            if (place == null)
            {
                return;
            }

            await _placeRepository.DeleteAsync(place);
        }

        public async Task DeleteCategory(Guid categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);

            if (category == null)
            {
                return;
            }

            await _categoryRepository.DeleteAsync(category);
        }
    }
}
