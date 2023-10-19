using keepscape_api.Services.Dashboards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}
