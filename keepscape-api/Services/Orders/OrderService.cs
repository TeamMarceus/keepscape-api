﻿using AutoMapper;
using keepscape_api.Dtos.Orders;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Interfaces;
using keepscape_api.Services.BaseImages;
using keepscape_api.Services.Paypal;

namespace keepscape_api.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrderPaymentRepository _orderPaymentRepository;
        private readonly IImageService _imageService;
        private readonly IPaypalService _paypalService;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository, 
            IUserRepository userRepository,
            IOrderPaymentRepository orderPaymentRepository,
            IImageService imageService,
            IPaypalService paypalService,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _orderPaymentRepository = orderPaymentRepository;
            _imageService = imageService;
            _paypalService = paypalService;
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

            await _orderRepository.UpdateAsync(order);

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

            if (order.Status == OrderStatus.AwaitingSeller)
            {
                order.Status = OrderStatus.Ongoing;
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

        public async Task<OrderBuyerResponseDto?> PayOrderBuyer(Guid userId, Guid orderId, Guid paypalOrderId)
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

            order.Status = OrderStatus.AwaitingSeller;

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

     
    }
}
