using keepscape_api.Models;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface IOrderReportRepository : IBaseRepository<OrderReport>
    {
        Task<OrderReport?> GetOrderReport(Guid orderId);
    }
}
