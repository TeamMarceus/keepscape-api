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

        // Buyers
        [HttpGet("buyers")]
        [Authorize(Policy = "Buyer")]
        public async Task<IActionResult> GetBuyerOrders([FromQuery] OrderQuery orderQuery)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }

                var buyerId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (buyerId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var orders = await _orderService.GetBuyerOrders(buyerId, orderQuery);

                if (orders == null)
                {
                    return BadRequest("Invalid credentials.");
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_orderService.GetBuyerOrders)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("buyers/count")]
        [Authorize(Policy = "Buyer")]
        public async Task<IActionResult> GetBuyerOrdersCount()
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }

                var buyerId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (buyerId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var ordersCount = await _orderService.GetBuyerOrdersCount(buyerId);

                return Ok(ordersCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_orderService.GetBuyerOrdersCount)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("buyers/{orderId}/confirm")]
        [Authorize(Policy = "Buyer")]
        public async Task<IActionResult> ConfirmOrderBuyer(Guid orderId)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }

                var buyerId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (buyerId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var order = await _orderService.ConfirmOrderBuyer(buyerId, orderId);

                if (order == null)
                {
                    return BadRequest("Invalid credentials.");
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_orderService.ConfirmOrderBuyer)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("buyers/{orderId}/cancel")]
        [Authorize(Policy = "Buyer")]
        public async Task<IActionResult> CancelOrderBuyer(Guid orderId)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }

                var buyerId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (buyerId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var order = await _orderService.CancelOrderBuyer(buyerId, orderId);

                if (order == null)
                {
                    return BadRequest("Invalid credentials.");
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_orderService.CancelOrderBuyer)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("buyers/{orderId}/pay")]
        [Authorize(Policy = "Buyer")]
        public async Task<IActionResult> PayOrderBuyer(Guid orderId, [FromBody] OrderPaypalDto orderPaypalDto)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }

                var buyerId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (buyerId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var order = await _orderService.PayOrderBuyer(buyerId, orderId, orderPaypalDto.PaypalOrderId);

                if (order == null)
                {
                    return BadRequest("Invalid credentials.");
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_orderService.PayOrderBuyer)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        // Sellers
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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromForm] OrderUpdateDeliveryFeeDto orderUpdateDeliveryFeeDto)
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
        public async Task<IActionResult> CancelOrderSeller(Guid orderId)
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

                var order = await _orderService.CancelOrderSeller(sellerId, orderId);

                if (order == null)
                {
                    return BadRequest("Invalid credentials.");
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_orderService.CancelOrderSeller)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("sellers/{orderId}/deliver")]
        public async Task<IActionResult> DeliverOrderSeller(Guid orderId)
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

                var order = await _orderService.DeliverOrderSeller(sellerId, orderId);

                if (order == null)
                {
                    return BadRequest("Invalid credentials.");
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_orderService.DeliverOrderSeller)} threw an exception");
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
