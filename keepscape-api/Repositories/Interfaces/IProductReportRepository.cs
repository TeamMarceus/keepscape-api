using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface IProductReportRepository : IBaseRepository<ProductReport>
    {
        Task<IEnumerable<ProductReport>> GetByProductIdAsync(Guid productId);
        Task<(IEnumerable<ProductReport> ProductReports, int PageCount)> Get(ProductReportQuery productReportQuery);
        Task<bool> UpdateRangeAsync(IEnumerable<ProductReport> productReports);
    }
}
