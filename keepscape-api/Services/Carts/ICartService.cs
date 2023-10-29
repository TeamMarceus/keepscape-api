using keepscape_api.Dtos.Carts;

namespace keepscape_api.Services.Carts
{
    public interface ICartService
    {
        Task<CartResponseDto?> AddProduct(Guid userId, CartRequestDto cartRequestDto);
        Task<CartResponseDto?> Get(Guid userId);
        Task<int> GetCount(Guid userId);
        Task<CartResponseDto?> Update(Guid userId, CartUpdateDto cartUpdateDto);
        Task<CartResponseDto?> Delete(Guid userId, IEnumerable<Guid> cartItemIds);
        Task<bool> Checkout(Guid userId);
    }
}
