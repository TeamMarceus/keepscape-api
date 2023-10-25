using AutoMapper;
using keepscape_api.Dtos.Carts;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace keepscape_api.Services.Carts
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CartService(
            ICartRepository cartRepository, 
            IOrderRepository orderRepository,
            IProductRepository productRepository, 
            IUserRepository userRepository, 
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
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

            var existingCartItem = cart.Items.Where(x => x.ProductId == cartRequestDto.ProductId);

            if (!existingCartItem.IsNullOrEmpty() && !existingCartItem.First().Product!.IsCustomizable)
            {
                existingCartItem.First().Quantity += cartRequestDto.Quantity;
            }
            else if (
                !existingCartItem.IsNullOrEmpty() && 
                existingCartItem.Any(x => x.Product!.IsCustomizable) &&
                existingCartItem.Any(x => x.CustomizationMessage == cartRequestDto.CustomizationMessage)
            ) 
            { 
                existingCartItem.First(x => x.CustomizationMessage == cartRequestDto.CustomizationMessage).Quantity += cartRequestDto.Quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductId = product.Id,
                    Quantity = cartRequestDto.Quantity,
                    CustomizationMessage = cartRequestDto.CustomizationMessage ?? ""
                };

                cart.Items.Add(cartItem);
            }

            await _cartRepository.UpdateAsync(cart);

            return await Get(userId);
        }

        public async Task<bool> Checkout(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            if (user.BuyerProfile == null)
            {
                return false;
            }

            var cart = await _cartRepository.GetCartByBuyerProfileId(user.BuyerProfile.Id);

            if (cart == null)
            {
                return false;
            }

            if (cart.Items.IsNullOrEmpty())
            {
                return false;
            }

            var sellerIdToItems = cart.Items.GroupBy(x => x.Product!.SellerProfile!.Id!).ToDictionary(x => x.Key, x => x.ToList());

            foreach (var sellerIdToItem in sellerIdToItems)
            {
                var sellerId = sellerIdToItem.Key;
                var items = sellerIdToItem.Value;

                var order = new Order
                {
                    BuyerProfile = user.BuyerProfile,
                    SellerProfileId = sellerId,
                    Items = items.Select(x => new OrderItem
                    {
                        Product = x.Product,
                        Quantity = x.Quantity,
                        CustomizationMessage = x.CustomizationMessage
                    }).ToList(),
                    TotalPrice = items.Sum(x => x.Product!.BuyerPrice * x.Quantity)
                };

                await _orderRepository.AddAsync(order);
            }

            cart.Items.Clear();

            return await _cartRepository.UpdateAsync(cart);
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

            var cartItem = cart.Items.SingleOrDefault(x => x.Id == cartItemId);

            if (cartItem == null)
            {
                return null;
            }

            cart.Items.Remove(cartItem);

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

            if (cart.Items.IsNullOrEmpty())
            {
                return new CartResponseDto
                {
                    Id = cart.Id,
                    CartSellers = new List<CartSellerDto>()
                };
            }

            var sellers = cart.Items.GroupBy(i => i.Product!.SellerProfileId).Select(g => g.First().Product!.SellerProfile);

            var cartSellers = new List<CartSellerDto>();

            foreach(var seller in sellers)
            {
                cartSellers.Add(new CartSellerDto
                {
                    Id = seller!.UserId,
                    SellerName = seller.Name,
                    CartItems = cart.Items.Where(x => x.Product!.SellerProfile!.UserId == seller.UserId).Select(x => _mapper.Map<CartItemResponseDto>(x))
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
            var cartItems = cart.Items.Where(x => cartItemDictKeys.Contains(x.Id));

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
