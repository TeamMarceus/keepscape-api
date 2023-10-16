using keepscape_api.Models;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByBuyerProfileId(Guid buyerProfileId);
        Task<IEnumerable<Order>> GetOrdersBySellerProfileId(Guid sellerProfileId);
        Task<Order?> CreateDeliveryLogByOrderId(Guid orderId, OrderDeliveryLog orderDeliveryLog);
    }
}
