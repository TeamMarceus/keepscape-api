using AutoMapper;
using keepscape_api.Dtos.Carts;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;

namespace keepscape_api.Services.Carts
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CartService(
            ICartRepository cartRepository, 
            IProductRepository productRepository, 
            IUserRepository userRepository, 
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<CartItemResponseDto?> AddProductToCart(Guid userId, CartRequestDto cartRequestDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            if (user.BuyerProfile == null)
            {
                return null;
            }

            var cart = await _cartRepository.GetByIdAsync(user.BuyerProfile.CartId);

            if (cart == null)
            {
                return null;
            }

            var product = await _productRepository.GetByIdAsync(cartRequestDto.ProductId);

            if (product == null)
            {
                return null;
            }

            cart.CartItems.Add(new CartItem
            {
                Product = product,
                Quantity = cartRequestDto.Quantity,
                CustomizationMessage = product.IsCustomizable ? cartRequestDto.CustomizationMessage ?? "" : string.Empty
            });

            await _cartRepository.UpdateAsync(cart);

            return _mapper.Map<CartItemResponseDto>(cart.CartItems.Last());
        }

        public Task<CartItemResponseDto?> Get(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
