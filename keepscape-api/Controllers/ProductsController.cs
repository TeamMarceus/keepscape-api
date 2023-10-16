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

                return CreatedAtRoute(nameof(GetById), new { productId = productResponseDto.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_productService.Create)} threw an exception");
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

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ProductQueryParameters productQueryParameters)
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

        [HttpGet("{productId}")]
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

        [HttpPost("{productId}/reviews")]
        [Authorize(Policy = "Buyer")]
        public async Task<IActionResult> AddReview(Guid productId, [FromBody] ProductReviewCreateDto productReviewCreateDto)
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


        [HttpPut("{productId}")]
        [Authorize(Policy = "Seller")]
        public async Task<IActionResult> Update(Guid productId, [FromBody] ProductUpdateDto productUpdateDto)
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
    }
}
