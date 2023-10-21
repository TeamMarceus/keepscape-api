using keepscape_api.Dtos.Orders;
using keepscape_api.Dtos.Products;
using keepscape_api.Dtos.Reports;

namespace keepscape_api.Services.Reports
{
    public interface IReportService
    {
        Task<IEnumerable<ReportProductResponseDto>> GetProductReports(Guid productId);
        Task<IEnumerable<ProductResponseAdminDto>> GetAllProductReports();
        Task<bool> ResolveProductReports(Guid productId);
        Task<bool> CreateProductReport(Guid productId, Guid userId, ReportProductRequestDto reportRequestDto);
        Task<ReportOrderResponseDto?> GetOrderReport(Guid orderId);
        Task<IEnumerable<OrderResponseAdminDto>> GetAllOrderReports();
        Task<bool> ResolveOrderReport(Guid orderId);
        Task<bool> RefundOrderReport(Guid orderId);
        Task<bool> CreateOrderReport(Guid orderId, Guid userId, ReportOrderRequestDto reportRequestDto);
    }
}
