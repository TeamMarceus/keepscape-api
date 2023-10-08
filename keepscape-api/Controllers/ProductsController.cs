using keepscape_api.QueryModels;
using keepscape_api.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace keepscape_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IProductService _productService;

        public ProductsController(ILogger<ProductService> logger, IProductService productService)
        {
                _logger = logger;
                _productService = productService;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetProductCategories()
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
                _logger.LogError(ex, "Error getting product categories.");

                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ProductQueryParameters? productQueryParameters)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (productQueryParameters == null)
                {
                    var products = await _productService.GetALl();

                    return Ok(products);
                }
                else if (productQueryParameters != null && productQueryParameters.Category == null)
                {
                    var products = await _productService.GetALl();

                    return Ok(products);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products.");

                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
