using keepscape_api.Dtos.Orders;
using keepscape_api.QueryModels;
using keepscape_api.Services.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace keepscape_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<IOrderService> _logger;

        public OrdersController(
            IOrderService orderService,
            ILogger<IOrderService> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet("sellers")]
        [Authorize(Policy = "Seller")]
        public async Task<IActionResult> GetSellerOrders([FromQuery] OrderQuery orderQuery)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }

                var sellerId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (sellerId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var orders = await _orderService.GetSellerOrders(sellerId, orderQuery);

                if (orders == null)
                {
                    return BadRequest("Invalud credentials.");
                }

                return Ok(orders);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_orderService.GetSellerOrders)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("sellers/{orderId}")]
        [Authorize(Policy = "Seller")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromBody] OrderUpdateDeliveryFeeDto orderUpdateDeliveryFeeDto)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }

                var sellerId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (sellerId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var order = await _orderService.UpdateDeliveryFee(sellerId, orderId, orderUpdateDeliveryFeeDto);

                if (order == null)
                {
                    return BadRequest("Invalid credentials.");
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_orderService.UpdateDeliveryFee)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("sellers/{orderId}/cancel")]
        [Authorize(Policy = "Seller")]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }

                var sellerId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (sellerId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var order = await _orderService.CancelOrder(sellerId, orderId);

                if (order == null)
                {
                    return BadRequest("Invalid credentials.");
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_orderService.CancelOrder)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

 

        [HttpPost("sellers/{orderId}/logs")]
        [Authorize(Policy = "Seller")]
        public async Task<IActionResult> AddOrderDeliveryLogs(Guid orderId, [FromBody] OrderAddDeliveryLogDto orderAddDeliveryLogDto)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }

                var sellerId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (sellerId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var order = await _orderService.CreateSellerOrderDeliveryLogs(sellerId, orderId, orderAddDeliveryLogDto);

                if (order == null)
                {
                    return BadRequest("Invalid credentials.");
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_orderService.CreateSellerOrderDeliveryLogs)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
