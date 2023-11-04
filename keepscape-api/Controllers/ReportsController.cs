using keepscape_api.Dtos.Reports;
using keepscape_api.QueryModels;
using keepscape_api.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace keepscape_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ILogger<IReportService> _logger;
        private readonly IReportService _reportService;

        public ReportsController(
                ILogger<IReportService> logger,
                IReportService reportService
            )
        {
            _logger = logger;
            _reportService = reportService;
        }

        [HttpPost("products/{productId}")]
        [Authorize(Policy = "Buyer")]
        public async Task<IActionResult> CreateProductReport(Guid productId, ReportRequestDto reportRequestDto)
        {
            try
            {
                var userId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (userId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var result = await _reportService.CreateProductReport(
                    productId, 
                    userId,
                    reportRequestDto);

                if (!result)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_reportService.CreateProductReport)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("products")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetProductWithReports([FromQuery] ProductReportQuery productReportQuery)
        {
            try
            {
                var productReports = await _reportService.GetAllProductWithReports(productReportQuery);

                return Ok(productReports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_reportService.GetAllProductWithReports)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("products/{productId}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetProductReports(Guid productId)
        {
            try
            {
                var productReports = await _reportService.GetProductReports(productId);

                if (productReports.IsNullOrEmpty())
                {
                    return NotFound();
                }

                return Ok(productReports);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_reportService.GetProductReports)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("products/{productId}/resolve")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ResolveProductWithReports(Guid productId)
        {
            try
            {
                var result = await _reportService.ResolveProductWithReports(productId);

                if (!result)
                {
                    return BadRequest("Product not found or product has no reports.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_reportService.ResolveProductWithReports)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("orders/{orderId}")]
        [Authorize(Policy = "Buyer")]
        public async Task<IActionResult> CreateOrderReport(Guid orderId, ReportRequestDto reportRequestDto)
        {
            try
            {
                var userId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (userId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var result = await _reportService.CreateOrderReport(orderId, userId, reportRequestDto);

                if (!result)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_reportService.CreateOrderReport)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("orders")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetOrderWithReports([FromQuery] OrderReportQuery orderReportQuery)
        {
            try
            {
                var orderReports = await _reportService.GetAllOrderWithReports(orderReportQuery);

                return Ok(orderReports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_reportService.GetAllOrderWithReports)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("orders/{orderId}/resolve")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ResolveOrderWithReport(Guid orderId)
        {
            try
            {
                var result = await _reportService.ResolveOrderWithReport(orderId);

                if (!result)
                {
                    return BadRequest("Order not found or order not eligible for resolving.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_reportService.ResolveOrderWithReport)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("orders/{orderId}/refund")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> RefundOrderWithReport(Guid orderId)
        {
            try
            {
                var result = await _reportService.RefundOrderWithReport(orderId);

                if (!result)
                {
                    return BadRequest("Order not found or order not eligible for refund.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_reportService.RefundOrderWithReport)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
