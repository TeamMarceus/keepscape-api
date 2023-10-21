using keepscape_api.Services.Dashboards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace keepscape_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardsController : ControllerBase
    {
        private readonly ILogger<DashboardService> _logger;
        private readonly IDashboardService _dashboardService;

        public DashboardsController(
            ILogger<DashboardService> logger,
            IDashboardService dashboardService)
        {
            _logger = logger;
            _dashboardService = dashboardService;
        }

        [HttpGet("admin")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            try
            {
                var dashboard = await _dashboardService.GetAdminDashboard();
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_dashboardService.GetAdminDashboard)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("seller")]
        [Authorize(Policy = "Seller")]
        public async Task<IActionResult> GetSellerDashboard()
        {
            try
            {
                var sellerId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (sellerId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var dashboard = await _dashboardService.GetSellerDashboard(sellerId);
                
                if (dashboard == null)
                {
                    return NotFound();
                }

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_dashboardService.GetSellerDashboard)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
