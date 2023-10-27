using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<IEnumerable<Order>> GetByBuyerProfileId(Guid buyerProfileId);
        Task<int> GetByBuyerProfileIdCount(Guid buyerProfileId);
        Task<IEnumerable<Order>> GetBySellerProfileId(Guid sellerProfileId);
        Task<IEnumerable<Order>> GetByProductId(Guid productId);
        Task<(IEnumerable<Order> Orders, int PageCount)> GetForSeller(Guid sellerProfileId, OrderQuery orderQuery);
        Task<(IEnumerable<Order> Orders, int PageCount)> GetForAdmin(OrderReportQuery orderReportQuery);
    }
}
