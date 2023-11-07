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

            if (product.Quantity < cartRequestDto.Quantity || product.IsHidden)
            {
                return null;
            }

            var existingCartItem = cart.Items.Where(x => x.ProductId == cartRequestDto.ProductId).SingleOrDefault();

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += cartRequestDto.Quantity;

                if (existingCartItem.Quantity > existingCartItem.Product!.Quantity)
                {
                    existingCartItem.Quantity = existingCartItem.Product!.Quantity;
                }

                if (existingCartItem.Product!.IsCustomizable)
                {
                    existingCartItem.CustomizationMessage = existingCartItem.CustomizationMessage + "\n\n" + cartRequestDto.CustomizationMessage;
                }
            }
            else
            {
                if (product.Quantity < cartRequestDto.Quantity)
                {
                    cartRequestDto.Quantity = product.Quantity;
                }

                if (product.IsHidden || product.DateTimeDeleted != null)
                {
                    return null;
                }

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

        public async Task<bool> Checkout(Guid userId, IEnumerable<Guid> cartItemIds)
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

            var cartItems = cart.Items.Where(x => cartItemIds.Contains(x.Id)).ToList();

            var sellerIdToItems = cartItems.GroupBy(x => x.Product!.SellerProfile!.Id!).ToDictionary(x => x.Key, x => x.ToList());

            foreach (var sellerIdToItem in sellerIdToItems)
            {

                var sellerId = sellerIdToItem.Key;
                var items = sellerIdToItem.Value;

                var order = new Order
                {
                    BuyerProfileId = user.BuyerProfile.Id,
                    SellerProfileId = sellerId,
                    Items = items.Where(x => x.Product!.Quantity > x.Quantity).Select(x => new OrderItem
                    {
                        ProductId = x.Product!.Id,
                        Quantity = x.Quantity,
                        CustomizationMessage = x.CustomizationMessage,
                        Price = x.Product!.BuyerPrice * x.Quantity
                    }).ToList(),
                    TotalPrice = items.Sum(x => x.Product!.BuyerPrice * x.Quantity)
                };

                await _orderRepository.AddAsync(order);
            }

            cart.Items = cart.Items.Where(x => !cartItemIds.Contains(x.Id)).ToList();

            return await _cartRepository.UpdateAsync(cart);
        }

        public async Task<CartResponseDto?> Delete(Guid userId, IEnumerable<Guid> cartItemIds)
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

            var cartItems = cart.Items.Where(x => cartItemIds.Contains(x.Id)).ToList();
            
            foreach(var cartItem in cartItems)
            {
                cart.Items.Remove(cartItem);
            }
            
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
            var hiddenItems = new List<CartSellerDto>();

            foreach(var seller in sellers)
            {
                if (seller == null)
                {
                    continue;
                }

                var cartItems = cart.Items.Where(x => x.Product!.SellerProfile!.UserId == seller.UserId && !x.Product.IsHidden).ToList();

                foreach(var cartItem in cartItems)
                {
                    if (cartItem.Product!.Quantity < cartItem.Quantity)
                    {
                        cartItem.Quantity = cartItem.Product!.Quantity;
                    }
                }

                cartSellers.Add(new CartSellerDto
                {
                    Id = seller!.UserId,
                    SellerName = seller.Name,
                    CartItems = cartItems.Select(x => _mapper.Map<CartItemResponseDto>(x))

                });

                var hiddenProducts = cart.Items.Where(x => x.Product!.SellerProfile!.UserId == seller.UserId && 
                                    (x.Product.IsHidden || x.Quantity <= 0)).Distinct()
                                    .ToList();

                if (hiddenProducts.IsNullOrEmpty())
                {
                    continue;
                }

                hiddenItems.Add(new CartSellerDto
                {
                    Id = seller!.UserId,
                    SellerName = seller.Name,
                    CartItems = hiddenProducts.Select(x => _mapper.Map<CartItemResponseDto>(x))
                });
            }
            
            var message = !hiddenItems.IsNullOrEmpty() ? "Some items are out of stock or unavailable" : null;
        
            return new CartResponseDto
            {
                Id = cart.Id,
                CartSellers = cartSellers,
                Message = message,
                HiddenItems = hiddenItems
            };
        }

        public async Task<int> GetCount(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return 0;
            }

            if (user.BuyerProfile == null)
            {
                return 0;
            }

            var cart = await _cartRepository.GetCartByBuyerProfileId(user.BuyerProfile.Id);

            if (cart == null)
            {
                return 0;
            }

            return cart.Items.Count;
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

                if (cartItem.Quantity <= 0)
                {
                    cart.Items.Remove(cartItem);
                    continue;
                }

                if (cartItem.Quantity > cartItem.Product!.Quantity)
                {
                    cartItem.Quantity = cartItem.Product!.Quantity;
                }
                
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
