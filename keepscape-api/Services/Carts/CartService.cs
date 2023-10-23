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
        public async Task<CartResponseDto?> AddProduct(Guid userId, CartRequestDto cartRequestDto)
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

            var cart = await _cartRepository.GetCartByBuyerProfileId(user.BuyerProfile.Id);

            if (cart == null)
            {
                return null;
            }

            var product = await _productRepository.GetByIdAsync(cartRequestDto.ProductId);

            if (product == null)
            {
                return null;
            }

            var existingCartItem = cart.CartItems.SingleOrDefault(x => x.ProductId == cartRequestDto.ProductId);

            if (existingCartItem != null && existingCartItem.CustomizationMessage == cartRequestDto.CustomizationMessage)
            {
                existingCartItem.Quantity += cartRequestDto.Quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    Product = product,
                    Quantity = cartRequestDto.Quantity
                };

                cart.CartItems.Add(cartItem);
            }

            await _cartRepository.UpdateAsync(cart);

            return await Get(userId);
        }

        public async Task<CartResponseDto?> Delete(Guid userId, Guid cartItemId)
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

            var cart = await _cartRepository.GetCartByBuyerProfileId(user.BuyerProfile.Id);

            if (cart == null)
            {
                return null;
            }

            var cartItem = cart.CartItems.SingleOrDefault(x => x.Id == cartItemId);

            if (cartItem == null)
            {
                return null;
            }

            cart.CartItems.Remove(cartItem);

            await _cartRepository.UpdateAsync(cart);

            return await Get(userId);
        }

        public async Task<CartResponseDto?> Get(Guid userId)
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

            var cart = await _cartRepository.GetCartByBuyerProfileId(user.BuyerProfile.Id);

            if (cart == null)
            {
                return null;
            }

            var sellers = cart.CartItems.GroupBy(i => i.Product!.SellerProfileId).Select(g => g.First().Product!.SellerProfile);

            var cartSellers = new List<CartSellerDto>();

            foreach(var seller in sellers)
            {
                cartSellers.Add(new CartSellerDto
                {
                    Id = seller!.UserId,
                    SellerName = seller.Name,
                    CartItems = cart.CartItems.Where(x => x.Product!.SellerProfile!.UserId == seller.UserId).Select(x => _mapper.Map<CartItemResponseDto>(x))
                });
            }
            
            return new CartResponseDto
            {
                Id = cart.Id,
                CartSellers = cartSellers
            };
        }

        public async Task<CartResponseDto?> Update(Guid userId, CartUpdateDto cartUpdateDto)
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

            var cart = await _cartRepository.GetCartByBuyerProfileId(user.BuyerProfile.Id);

            if (cart == null)
            {
                return null;
            }

            var cartItemDictKeys = cartUpdateDto.CartItems.Select(x => x.Key).ToList();
            var cartItems = cart.CartItems.Where(x => cartItemDictKeys.Contains(x.Id));

            foreach (var cartItem in cartItems)
            {
                var cartItemUpdate = cartUpdateDto.CartItems[cartItem.Id];

                if (cartItemUpdate == null)
                {
                    continue;
                }
                cartItem.Quantity = cartItemUpdate.Quantity != null ? cartItemUpdate.Quantity.Value : cartItem.Quantity;
                
                if (cartItem.Product!.IsCustomizable)
                {
                    cartItem.CustomizationMessage = cartItemUpdate.CustomizationMessage ?? cartItem.CustomizationMessage;
                }
            }

            await _cartRepository.UpdateAsync(cart);

            return await Get(userId);
        }
    }
}
