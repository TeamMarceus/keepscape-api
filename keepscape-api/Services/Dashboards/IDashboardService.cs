using keepscape_api.Dtos.Dashboards.Admin;
using keepscape_api.Dtos.Dashboards.Seller;

namespace keepscape_api.Services.Dashboards
{
    public interface IDashboardService
    {
        Task<AdminDashboardResponseDto> GetAdminDashboard();
        Task<SellerDashboardResponseDto?> GetSellerDashboard(Guid sellerId); 
    }
}
