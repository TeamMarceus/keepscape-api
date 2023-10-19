using keepscape_api.Models;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<IEnumerable<Order>> GetByBuyerProfileId(Guid buyerProfileId);
        Task<IEnumerable<Order>> GetBySellerProfileId(Guid sellerProfileId);
    }
}
