using keepscape_api.Dtos.Carts;
using keepscape_api.Services.Carts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace keepscape_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<ICartService> _logger;

        public CartsController(
                ICartService cartService,
                ILogger<ICartService> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "Buyer")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (userId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var cart = await _cartService.Get(userId);

                if (cart == null)
                {
                    return NotFound();
                }

                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_cartService.Get)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Policy = "Buyer")]
        public async Task<IActionResult> AddProduct([FromBody]CartRequestDto cartRequestDto)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }
                var userId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (userId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var cartItem = await _cartService.AddProduct(userId, cartRequestDto);

                if (cartItem == null)
                {
                    return BadRequest("Invalid credentials.");
                }

                return Ok(cartItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_cartService.AddProduct)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut]
        [Authorize(Policy = "Buyer")]
        public async Task<IActionResult> Update([FromBody]CartUpdateDto cartUpdateDto)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }

                var userId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (userId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var cart = await _cartService.Update(userId, cartUpdateDto);

                if (cart == null)
                {
                    return BadRequest("Invalid credentials.");
                }

                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_cartService.Update)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{cartItemId}")]
        [Authorize(Policy = "Buyer")]
        public async Task<IActionResult> Delete(Guid cartItemId)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }

                var userId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (userId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var cart = await _cartService.Delete(userId, cartItemId);

                if (cart == null)
                {
                    return BadRequest("Invalid credentials.");
                }

                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_cartService.Delete)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
