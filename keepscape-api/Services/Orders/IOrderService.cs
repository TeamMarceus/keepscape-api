using keepscape_api.Dtos.Orders;
using keepscape_api.QueryModels;

namespace keepscape_api.Services.Orders
{
    public interface IOrderService
    {
        Task<OrderSellerResponseDto?> CreateSellerOrderDeliveryLogs(Guid userId, Guid orderId, OrderAddDeliveryLogDto orderDeliveryLogRequestDto);
        Task<OrderSellerResponsePaginatedDto?> GetSellerOrders(Guid userId, OrderQuery orderQuery);
        Task<int> GetBuyerOrdersCount(Guid userId);
        Task<OrderBuyerResponsePaginatedDto?> GetBuyerOrders(Guid userId, OrderQuery orderQuery);
        Task<OrderSellerResponseDto?> UpdateDeliveryFee(Guid userId, Guid orderId, OrderUpdateDeliveryFeeDto orderUpdateDeliveryFeeDto);
        Task<OrderBuyerResponseDto?> PayOrderBuyer(Guid userId, Guid orderId, string paypalOrderId);
        Task<OrderSellerResponseDto?> CancelOrderSeller(Guid userId, Guid orderId);
        Task<OrderSellerResponseDto?> DeliverOrderSeller(Guid userId, Guid orderId);
        Task<OrderBuyerResponseDto?> ConfirmOrderBuyer(Guid userId, Guid orderId);
        Task<OrderBuyerResponseDto?> CancelOrderBuyer(Guid userId, Guid orderId);
    }
}
