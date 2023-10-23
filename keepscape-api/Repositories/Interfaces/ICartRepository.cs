using keepscape_api.Models;

using keepscape_api.Repositories.Generics;
namespace keepscape_api.Repositories.Interfaces
{
    public interface ICartRepository : IBaseRepository<Cart>
    {
        Task<Cart?> GetCartByBuyerProfileId(Guid buyerProfileId);
    }
}
