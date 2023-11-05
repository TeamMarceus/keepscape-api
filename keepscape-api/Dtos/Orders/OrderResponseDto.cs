using keepscape_api.Dtos.Reports;
using keepscape_api.Enums;

namespace keepscape_api.Dtos.Orders
{
    public record OrderResponseDto
    {
    }

    public record OrderItemResponseDto
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public string ProductImageUrl { get; init; } = string.Empty;
        public string CustomizationMessage { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public decimal Price { get; init; }
    }

    public record OrderAdminResponseDto
    {
        public Guid Id { get; init; }
        public OrderSellerDto Seller { get; init; } = null!;
        public OrderBuyerDto Buyer { get; init; } = null!;
        public ReportOrderResponseDto Report { get; init; } = null!;
        public DateTime DateTimeCreated { get; init; }
        public IEnumerable<OrderItemResponseDto> Items { get; init; } = new List<OrderItemResponseDto>();
        public IEnumerable<OrderDeliveryLogDto> DeliveryLogs { get; init; } = new List<OrderDeliveryLogDto>();
        public string? DeliveryFeeProofImageUrl { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; init; } = OrderStatus.Pending.ToString();
    }

    public record OrderBuyerDto
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public string Preferences { get; init; } = string.Empty;
        public string Interests { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string DeliveryFullName { get; init; } = string.Empty;
        public string DeliveryAddress { get; init; } = string.Empty;
        public string AltMobileNumber { get; init; } = string.Empty;
    }

    public record OrderSellerDto
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public string IdImageUrl { get; init; } = string.Empty;
        public string BusinessPermitUrl { get; init; } = string.Empty;
        public string SellerName { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
    }

    public record OrderDeliveryLogDto
    {
        public DateTime DateTime { get; init; }
        public string Log { get; init; } = string.Empty;
    }
    public record OrderSellerResponseDto
    {
        public Guid Id { get; init; }
        public DateTime DateTimeCreated { get; init; }
        public OrderBuyerDto Buyer { get; init; } = null!;
        public IEnumerable<OrderItemResponseDto> Items { get; init; } = new List<OrderItemResponseDto>();
        public IEnumerable<OrderDeliveryLogDto> DeliveryLogs { get; init; } = new List<OrderDeliveryLogDto>();
        public string? DeliveryFeeProofImageUrl { get; init; }
        public decimal DeliveryFee { get; init; }
        public decimal TotalPrice { get; init; }
        public string Status { get; init; } = OrderStatus.Pending.ToString();
    }

    public record OrderBuyerResponseDto
    {
        public Guid Id { get; init; }
        public DateTime DateTimeCreated { get; init; }
        public OrderSellerDto Seller { get; init; } = null!;
        public IEnumerable<OrderItemResponseDto> Items { get; init; } = new List<OrderItemResponseDto>();
        public IEnumerable<OrderDeliveryLogDto> DeliveryLogs { get; init; } = new List<OrderDeliveryLogDto>();
        public string? DeliveryFeeProofImageUrl { get; init; }
        public decimal DeliveryFee { get; init; }
        public decimal TotalPrice { get; init; }
        public string DeliveryFullName { get; init; } = string.Empty;
        public string DeliveryAddress { get; init; } = string.Empty;
        public string AltMobileNumber { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public string Status { get; init; } = OrderStatus.Pending.ToString();
    }

    public record OrderAdminResponsePaginatedDto
    {
        public IEnumerable<OrderAdminResponseDto> Orders { get; init; } = new List<OrderAdminResponseDto>();
        public int PageCount { get; init; } = 0;
    }
    public record OrderSellerResponsePaginatedDto
    {
        public IEnumerable<OrderSellerResponseDto> Orders { get; init; } = new List<OrderSellerResponseDto>();
        public int PageCount { get; init; } = 0;
    }
    public record OrderBuyerResponsePaginatedDto
    {
        public IEnumerable<OrderBuyerResponseDto> Orders { get; init; } = new List<OrderBuyerResponseDto>();
        public int PageCount { get; init; } = 0;
    }
}
