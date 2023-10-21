using keepscape_api.Dtos.Reports;
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
        public async Task<IActionResult> GetProductReports()
        {
            try
            {
                var productReports = await _reportService.GetAllProductReports();

                if (productReports.IsNullOrEmpty())
                {
                    return NotFound();
                }

                return Ok(productReports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_reportService.GetAllProductReports)} threw an exception");
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
        public async Task<IActionResult> ResolveProductReports(Guid productId)
        {
            try
            {
                var result = await _reportService.ResolveProductReports(productId);

                if (!result)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_reportService.ResolveProductReports)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("orders")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetOrderReports()
        {
            try
            {
                var orderReports = await _reportService.GetAllOrderReports();

                if (orderReports.IsNullOrEmpty())
                {
                    return NotFound();
                }

                return Ok(orderReports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_reportService.GetAllOrderReports)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("orders/{orderId}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetOrderReports(Guid orderId)
        {
            try
            {
                var orderReport = await _reportService.GetOrderReport(orderId);

                if (orderReport == null)
                {
                    return NotFound();
                }

                return Ok(orderReport);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_reportService.GetOrderReport)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("orders/{orderId}/resolve")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ResolveOrderReport(Guid orderId)
        {
            try
            {
                var result = await _reportService.ResolveOrderReport(orderId);

                if (!result)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_reportService.ResolveOrderReport)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("orders/{orderId}/refund")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> RefundOrderReport(Guid orderId)
        {
            try
            {
                var result = await _reportService.RefundOrderReport(orderId);

                if (!result)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_reportService.RefundOrderReport)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
