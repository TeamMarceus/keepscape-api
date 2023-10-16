using keepscape_api.Models;

using keepscape_api.Repositories.Generics;
namespace keepscape_api.Repositories.Interfaces
{
    public interface ICartRepository : IBaseRepository<Cart>
    {
        Task<Cart?> CreateCartItemByCartId(Guid cartId, CartItem cartItem);
        Task<Cart?> GetCartByUserGuid(Guid buyerProfileId);
    }
}
