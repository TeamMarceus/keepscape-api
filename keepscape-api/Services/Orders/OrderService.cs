using AutoMapper;
using keepscape_api.Dtos.Orders;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Interfaces;
using keepscape_api.Services.BaseImages;
using keepscape_api.Services.OpenAI;
using keepscape_api.Services.Paypal;

namespace keepscape_api.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrderPaymentRepository _orderPaymentRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly IImageService _imageService;
        private readonly IPaypalService _paypalService;
        private readonly IOpenAIService _openAIService;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository, 
            IUserRepository userRepository,
            IOrderPaymentRepository orderPaymentRepository,
            IOrderItemRepository orderItemRepository,
            IBalanceRepository balanceRepository,
            IImageService imageService,
            IPaypalService paypalService,
            IOpenAIService openAIService,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _orderPaymentRepository = orderPaymentRepository;
            _orderItemRepository = orderItemRepository;
            _balanceRepository = balanceRepository;
            _imageService = imageService;
            _paypalService = paypalService;
            _openAIService = openAIService;
            _mapper = mapper;
        }

        public async Task<OrderBuyerResponseDto?> CancelOrderBuyer(Guid userId, Guid orderId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (user == null || order == null)
            {
                return null;
            }

            var buyerProfileId = user.BuyerProfile != null ? user.BuyerProfile.Id : Guid.Empty;

            if (buyerProfileId == Guid.Empty || order.BuyerProfileId != buyerProfileId)
            {
                return null;
            }

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.AwaitingBuyer)
            {
                return null;
            }

            order.Status = OrderStatus.Cancelled;

            foreach (var orderItem in order.Items)
            {
                var product = orderItem.Product;

                if (product == null) { continue; }

                product.Quantity += orderItem.Quantity;
            }

            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderBuyerResponseDto>(order);
        }

        public async Task<OrderSellerResponseDto?> CancelOrderSeller(Guid userId, Guid orderId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (user == null || order == null)
            {
                return null;
            }

            var sellerProfileId = user.SellerProfile != null ? user.SellerProfile.Id : Guid.Empty;

            if (sellerProfileId == Guid.Empty || order.SellerProfileId != sellerProfileId)
            {
                return null;
            }

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.AwaitingBuyer)
            {
                return null;
            }

            order.Status = OrderStatus.Cancelled;

            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderSellerResponseDto>(order);
        }

        public async Task<OrderBuyerResponseDto?> ConfirmOrderBuyer(Guid userId, Guid orderId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (user == null || order == null)
            {
                return null;
            }

            var buyerProfileId = user.BuyerProfile != null ? user.BuyerProfile.Id : Guid.Empty;

            if (buyerProfileId == Guid.Empty || order.BuyerProfileId != buyerProfileId)
            {
                return null;
            }

            if (order.Status != OrderStatus.AwaitingConfirmation && order.Status != OrderStatus.Reported)
            {
                return null;
            }

            order.Status = OrderStatus.Delivered;

            var seller = order.SellerProfile!.User!;

            var sellerBalance = await _balanceRepository.GetByUserId(seller.Id);

            if (sellerBalance == null) { return null; }

            var totalSellerPrice = order.Items.Sum(x => x.Product!.SellerPrice * x.Quantity);
            var totalDeliveryFee = order.DeliveryFee;
            var totalAmount = totalSellerPrice + totalDeliveryFee;

            sellerBalance.Amount += totalAmount;
            sellerBalance.Histories.Add(new BalanceLog
            {
                Amount = totalAmount,
                Remarks = $"Order with ID {order.Id} successfully delivered!"
            });

            await _orderRepository.UpdateAsync(order);
            await _balanceRepository.UpdateAsync(sellerBalance);

            return _mapper.Map<OrderBuyerResponseDto>(order);
        }

        public async Task<OrderSellerResponseDto?> CreateSellerOrderDeliveryLogs(Guid userId, Guid orderId, OrderAddDeliveryLogDto orderDeliveryLogRequestDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (user == null || order == null)
            {
                return null;
            }

            var sellerProfileId = user.SellerProfile != null ? user.SellerProfile.Id : Guid.Empty;

            if (sellerProfileId == Guid.Empty || order.SellerProfileId != sellerProfileId)
            {
                return null;
            }

            if (order.Status != OrderStatus.Ongoing)
            {
                return null;
            }
            var dateTime = orderDeliveryLogRequestDto.DateTime;

            if (dateTime > DateTime.UtcNow)
            {
                return null;
            }

            order.DeliveryLogs.Add(new OrderDeliveryLog
            {
                Log = orderDeliveryLogRequestDto.Log,
                DateTime = dateTime
            });

            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderSellerResponseDto>(order);
        }

        public async Task<OrderSellerResponseDto?> DeliverOrderSeller(Guid userId, Guid orderId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (user == null || order == null)
            {
                return null;
            }

            var sellerProfileId = user.SellerProfile != null ? user.SellerProfile.Id : Guid.Empty;

            if (sellerProfileId == Guid.Empty || order.SellerProfileId != sellerProfileId)
            {
                return null;
            }

            if (order.Status != OrderStatus.Ongoing)
            {
                return null;
            }

            order.Status = OrderStatus.AwaitingConfirmation;
            order.DeliveryLogs.Add(new OrderDeliveryLog
            {
                Log = "Marked as Delivered",
                DateTime = DateTime.UtcNow
            });

            foreach (var orderItem in order.Items)
            {
                var product = orderItem.Product;

                if (product == null) { continue; }

                product.TotalSold += orderItem.Quantity;
            }

            await _orderRepository.UpdateAsync(order);


            return _mapper.Map<OrderSellerResponseDto>(order);
        }

        public async Task<OrderBuyerResponsePaginatedDto?> GetBuyerOrders(Guid userId, OrderQuery orderQuery)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var buyerProfileId = user.BuyerProfile != null ? user.BuyerProfile.Id : Guid.Empty;

            if (buyerProfileId == Guid.Empty)
            {
                return null;
            }

            var (orders, pageCount) = await _orderRepository.GetForBuyer(buyerProfileId, orderQuery);
            var orderBuyerResponseDtos = _mapper.Map<IEnumerable<OrderBuyerResponseDto>>(orders);

            return new OrderBuyerResponsePaginatedDto
            {
                Orders = orderBuyerResponseDtos,
                PageCount = pageCount
            };
        }

        public async Task<int> GetBuyerOrdersCount(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return 0;
            }

            var buyerProfileId = user.BuyerProfile != null ? user.BuyerProfile.Id : Guid.Empty;

            if (buyerProfileId == Guid.Empty)
            {
                return 0;
            }

            return await _orderRepository.GetByBuyerProfileIdCount(buyerProfileId);
        }

        public async Task<OrderSellerResponsePaginatedDto?> GetSellerOrders(Guid userId, OrderQuery orderQuery)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var sellerProfileId = user.SellerProfile != null ? user.SellerProfile.Id : Guid.Empty;

            if (sellerProfileId == Guid.Empty)
            {
                return null;
            }

            var (orders, pageCount) = await _orderRepository.GetForSeller(sellerProfileId, orderQuery);
            var orderSellerResponseDtos = _mapper.Map<IEnumerable<OrderSellerResponseDto>>(orders);
            return new OrderSellerResponsePaginatedDto
            {
                Orders = orderSellerResponseDtos,
                PageCount = pageCount
            };
        }

        public async Task<OrderBuyerResponseDto?> PayOrderBuyer(Guid userId, Guid orderId, string paypalOrderId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (user == null || order == null)
            {
                return null;
            }

            var buyerProfileId = user.BuyerProfile != null ? user.BuyerProfile.Id : Guid.Empty;

            if (buyerProfileId == Guid.Empty || order.BuyerProfileId != buyerProfileId)
            {
                return null;
            }

            if (order.Status != OrderStatus.AwaitingBuyer)
            {
                return null;
            }

            var orderPaymentExists = await _orderPaymentRepository.GetOrderPaymentByPaymentMethodOrderId(paypalOrderId, PaymentMethod.Paypal);

            if (orderPaymentExists != null)
            {
                return null;
            }

            var isPaypalPaymentValid = await _paypalService.ValidatePaypalPayment(userId, paypalOrderId);

            if (!isPaypalPaymentValid)
            {
                return null;
            }

            var orderPayment = new OrderPayment
            {
                OrderId = order.Id,
                PaymentMethod = PaymentMethod.Paypal,
                PaymentMethodOrderId = paypalOrderId.ToString()
            };

            order.Status = OrderStatus.Ongoing;

            foreach (var orderItem in order.Items)
            {
                orderItem.Product!.Quantity -= orderItem.Quantity;
            }

            await _orderRepository.UpdateAsync(order);
            await _orderPaymentRepository.AddAsync(orderPayment);

            return _mapper.Map<OrderBuyerResponseDto>(order);
        }

        public async Task<OrderSellerResponseDto?> UpdateDeliveryFee(Guid userId, Guid orderId, OrderUpdateDeliveryFeeDto orderUpdateDeliveryFeeDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (user == null || order == null)
            {
                return null;
            }

            var sellerProfileId = user.SellerProfile != null ? user.SellerProfile.Id : Guid.Empty;

            if (sellerProfileId == Guid.Empty || order.SellerProfileId != sellerProfileId)
            {
                return null;
            }

            if (order.Status != OrderStatus.Pending)
            {
                return null;
            }

            order.TotalPrice += orderUpdateDeliveryFeeDto.DeliveryFee;
            order.DeliveryFee = orderUpdateDeliveryFeeDto.DeliveryFee;
            order.DeliveryFeeProofImageUrl = await _imageService.Upload("delivery-proof", orderUpdateDeliveryFeeDto.DeliveryFeeProofImage);
            order.Status = OrderStatus.AwaitingBuyer;

            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderSellerResponseDto>(order);
        }

        public async Task<bool> GenerateGiftMessage(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null) 
            { 
                return false; 
            }

            if (order.Status == OrderStatus.Pending || order.Status == OrderStatus.AwaitingBuyer || order.Status == OrderStatus.Cancelled)
            {
                return false;
            }

            var basePrompt = "Generate a one paragraph gift message for anyone who receives this gift. " +
                "The message must relate to Region 7, the place, and the personality of the person. " +
                "It must explain why the product is the perfect gift for them to enjoy and they must visit the place soon. " +
                "There is no special occasion, just a gift." +
                "Here are the following context about the product, and the person:\n";
            var baseUrl = "https://api.qrserver.com/v1/create-qr-code/?size=300x300&data=https://keepscape-app.vercel.app/gifts/";

            foreach (var orderItem in order.Items)
            {
                if (orderItem.Gift != null) { continue; }

                var product = orderItem.Product;

                if (product == null) { continue; }

                var categories = product.Categories;
                var place = product.Place;

                if (place == null) { continue; }

                var buyerProfile = order.BuyerProfile!;

                var prompt = basePrompt +
                    $"Product name: {orderItem.Product!.Name}, it belongs to the categories {string.Join(", ", categories.Select(x => x.Name))}\n" +
                    $"Place name: {place!.Name}\n" +
                    $"Interests of the buyer: {buyerProfile.Interests}\n" + 
                    $"Personality of the buyer: {buyerProfile.Description}\n" +
                    $"Preferences of the buyer: {buyerProfile.Preferences}";

                var result = await _openAIService.Prompt(prompt);

                if (result != null)
                {
                    var message = result.Split("\n").Where(x => x.Length > 50).FirstOrDefault();

                    if (message == null) { continue; }

                    orderItem.Gift = new OrderItemGift
                    {
                        Message = message,
                        PlaceId = place!.Id,
                        UserId = buyerProfile.UserId,
                        OrderItemId = orderItem.Id
                    };

                    orderItem.QRImageUrl = baseUrl + orderItem.Id;
                }
            }

            return await _orderRepository.UpdateAsync(order);
        }

        public async Task<OrderItemGiftDto?> GetGiftMessage(Guid orderItemId)
        {
            var orderItem = await _orderItemRepository.GetByIdAsync(orderItemId);

            if (orderItem == null) { return null; }

            var orderItemGift = orderItem.Gift;

            if (orderItemGift == null) 
            {
                var orderItemGenerated = await GenerateGiftMessageSingle(orderItem.OrderId, orderItemId);

                if (orderItemGenerated == null) { return null; }

                return _mapper.Map<OrderItemGiftDto>(orderItemGenerated.Gift);
            }

            return _mapper.Map<OrderItemGiftDto>(orderItemGift);
        }

        private async Task<OrderItem?> GenerateGiftMessageSingle(Guid orderId, Guid orderItemId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null) { return null; }

            var orderItem = order.Items.FirstOrDefault(x => x.Id == orderItemId);

            if (orderItem == null) { return null; }

            var product = orderItem.Product;

            if (product == null) { return null; }

            var categories = product.Categories;
            var place = product.Place;

            if (place == null) { return null; }

            var buyerProfile = order.BuyerProfile!;

            var prompt = $"Generate a one paragraph gift message for anyone who receives this gift. " +
                $"The message must relate to Region 7, the place, and the personality of the person. " +
                $"It must explain why the product is the perfect gift for them to enjoy and they must visit the place soon. " +
                $"There is no special occasion, just a gift." +
                $"Here are the following context about the product, and the person:\n" +
                $"Product name: {orderItem.Product!.Name}, it belongs to the categories {string.Join(", ", categories.Select(x => x.Name))}\n" +
                $"Place name: {place!.Name}\n" +
                $"Interests of the buyer: {buyerProfile.Interests}\n" +
                $"Personality of the buyer: {buyerProfile.Description}\n" +
                $"Preferences of the buyer: {buyerProfile.Preferences}";

            var result = await _openAIService.Prompt(prompt);

            if (result != null)
            {
                var message = result.Split("\n").Where(x => x.Length > 50).FirstOrDefault();

                if (message == null) { return null; }

                orderItem.Gift = new OrderItemGift
                {
                    Message = message,
                    PlaceId = place!.Id,
                    UserId = buyerProfile.UserId,
                    OrderItemId = orderItem.Id
                };

                orderItem.QRImageUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&data=https://keepscape-app.vercel.app/gifts/{orderItem.Id}";
            }

            await _orderRepository.UpdateAsync(order);

            order = await _orderRepository.GetByIdAsync(orderId);

            return orderItem;
        }
    }
}
