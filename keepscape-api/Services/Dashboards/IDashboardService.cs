using keepscape_api.Dtos.Dashboards.Admin;

namespace keepscape_api.Services.Dashboards
{
    public interface IDashboardService
    {
        Task<AdminDashboardResponseDto> GetAdminDashboard();
    }
}
