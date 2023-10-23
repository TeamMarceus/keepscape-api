using keepscape_api.Dtos.Orders;
using keepscape_api.Dtos.Products;
using keepscape_api.Dtos.Reports;
using keepscape_api.QueryModels;

namespace keepscape_api.Services.Reports
{
    public interface IReportService
    {
        // Products
        Task<IEnumerable<ReportProductResponseDto>> GetProductReports(Guid productId);
        Task<ProductResponseAdminPaginatedDto> GetAllProductWithReports(ProductReportQuery productReportQuery);
        Task<bool> ResolveProductWithReports(Guid productId);
        Task<bool> CreateProductReport(Guid productId, Guid userId, ReportRequestDto reportRequestDto);

        // Orders
        Task<OrderAdminResponsePaginatedDto> GetAllOrderWithReports(OrderReportQuery orderReportQuery);
        Task<bool> ResolveOrderWithReport(Guid orderId);
        Task<bool> RefundOrderWithReport(Guid orderId);
        Task<bool> CreateOrderReport(Guid orderId, Guid userId, ReportRequestDto reportRequestDto);
    }
}
