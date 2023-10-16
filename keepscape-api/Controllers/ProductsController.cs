using keepscape_api.Dtos.Products;
using keepscape_api.Enums;
using keepscape_api.QueryModels;
using keepscape_api.Services.Products;
using keepscape_api.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace keepscape_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public ProductsController(
            ILogger<ProductService> logger, 
            IProductService productService,
            IUserService userService
            )
        {
            _logger = logger;
            _productService = productService;
            _userService = userService;
        }

        // Products 
        [HttpPost]
        [Authorize(Policy = "Seller")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ProductCreateDto productCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var sellerId = Guid.TryParse(User.FindFirstValue("UserId"), out Guid sellerIdParsed) ? sellerIdParsed : Guid.Empty;

                if (sellerId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var userStatus = await _userService.GetStatus(sellerId);

                if (userStatus == UserStatus.Banned)
                {
                    return Forbid("User is banned.");
                }

                if (userStatus == UserStatus.NotFound)
                {
                    return BadRequest("User not found.");
                }

                if (userStatus == UserStatus.Pending)
                {
                    return Unauthorized("User is pending.");
                }

                var productResponseDto = await _productService.Create(sellerId, productCreateDto);

                if (productResponseDto == null)
                {
                    return BadRequest("Invalid credentials.");
                }

                return Ok(productResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.Create)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ProductQuery productQueryParameters)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var products = await _productService.Get(productQueryParameters);

                if (products == null)
                {
                    return NoContent();
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.Get)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{productId}", Name = nameof(GetById))]
        public async Task<IActionResult> GetById(Guid productId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var product = await _productService.GetById(productId);

                if (product == null)
                {
                    return NoContent();
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.GetById)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{productId}")]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = "Seller")]
        public async Task<IActionResult> Update(Guid productId, [FromForm] ProductUpdateDto productUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var sellerId = Guid.TryParse(User.FindFirstValue("UserId"), out Guid sellerIdParsed) ? sellerIdParsed : Guid.Empty;

                if (sellerId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var productUpdated = await _productService.Update(sellerId, productId, productUpdateDto);

                if (!productUpdated)
                {
                    return BadRequest("Product cannot be updated.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.Update)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{productId}")]
        [Authorize(Policy = "Seller")]
        public async Task<IActionResult> Delete(Guid productId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var sellerId = Guid.TryParse(User.FindFirstValue("UserId"), out Guid sellerIdParsed) ? sellerIdParsed : Guid.Empty;

                if (sellerId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                await _productService.Delete(sellerId, productId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.Delete)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        // Categories
        [HttpPost("categories")]
        [Authorize(Policy = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateCategory([FromForm] ProductCategoryPlaceCreateDto productCategoryPlaceCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var productCategoryPlaceDto = await _productService.CreateCategory(productCategoryPlaceCreateDto);

                if (productCategoryPlaceDto == null)
                {
                    return BadRequest("Product category cannot be added.");
                }

                return Ok(productCategoryPlaceDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.CreateCategory)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var productCategories = await _productService.GetProductCategories();

                if (productCategories == null)
                {
                    return NoContent();
                }

                return Ok(productCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.GetProductCategories)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("categories/{categoryId}")]
        [Authorize(Policy = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromForm] IFormFile image)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var categoryUpdated = await _productService.UpdateCategory(categoryId, image);

                if (!categoryUpdated)
                {
                    return BadRequest("Product category cannot be updated.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.UpdateCategory)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("categories/{categoryId}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _productService.DeleteCategory(categoryId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.DeleteCategory)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        // Places
        [HttpPost("places")]
        [Authorize(Policy = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePlace([FromForm] ProductCategoryPlaceCreateDto productCategoryPlaceCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var productCategoryPlaceDto = await _productService.CreatePlace(productCategoryPlaceCreateDto);

                if (productCategoryPlaceDto == null)
                {
                    return BadRequest("Product place cannot be added.");
                }

                return Ok(productCategoryPlaceDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.CreatePlace)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("places")]
        public async Task<IActionResult> GetPlaces()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var places = await _productService.GetPlaceCategories();

                if (places == null)
                {
                    return NoContent();
                }

                return Ok(places);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.GetPlaceCategories)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("places/{placeId}")]
        [Authorize(Policy = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdatePlace(Guid placeId, [FromForm] IFormFile image)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var placeUpdated = await _productService.UpdatePlace(placeId, image);

                if (!placeUpdated)
                {
                    return BadRequest("Product place cannot be updated.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.UpdatePlace)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("places/{placeId}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeletePlace(Guid placeId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _productService.DeletePlace(placeId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.DeletePlace)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        // Reviews

        [HttpPost("{productId}/reviews")]
        [Authorize(Policy = "Buyer")]
        public async Task<IActionResult> CreateReview(Guid productId, [FromBody] ProductReviewCreateDto productReviewCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var buyerId = Guid.TryParse(User.FindFirstValue("UserId"), out Guid buyerIdParsed) ? buyerIdParsed : Guid.Empty;

                if (buyerId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var productReview = await _productService.CreateReview(buyerId, productId, productReviewCreateDto);

                if (!productReview)
                {
                    return BadRequest("Product review cannot be added.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.CreateReview)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{productId}/reviews")]
        [Authorize(Policy = "Buyer")]
        public async Task<IActionResult> GetReview(Guid productId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = Guid.TryParse(User.FindFirstValue("UserId"), out Guid userIdParsed) ? userIdParsed : Guid.Empty;

                if (userId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var productReview = await _productService.GetReview(userId, productId);

                if (productReview == null)
                {
                    return NoContent();
                }

                return Ok(productReview);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.GetReview)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{productId}/reviews")]
        [Authorize(Policy = "Buyer")]
        public async Task<IActionResult> UpdateReview(Guid productId, [FromBody] ProductReviewUpdateDto productReviewUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = Guid.TryParse(User.FindFirstValue("UserId"), out Guid userIdParsed) ? userIdParsed : Guid.Empty;

                if (userId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var productReview = await _productService.UpdateReview(userId, productId, productReviewUpdateDto);

                if (!productReview)
                {
                    return BadRequest("Product review cannot be updated.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.UpdateReview)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{productId}/reviews/{reviewId}")]
        [Authorize(Policy = "Buyer")]
        public async Task<IActionResult> DeleteReview(Guid reviewId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _productService.DeleteReview(reviewId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.DeleteReview)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
