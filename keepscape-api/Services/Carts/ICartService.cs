using keepscape_api.Dtos.Carts;

namespace keepscape_api.Services.Carts
{
    public interface ICartService
    {
        Task<CartItemResponseDto?> AddProductToCart(Guid userId, CartRequestDto cartRequestDto);
        Task<CartItemResponseDto?> Get(Guid userId);
    }
}
