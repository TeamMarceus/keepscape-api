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
        public string CustomizedMessage { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public decimal Price { get; init; }
    }

    public record OrderAdminResponseDto
    {
        public Guid Id { get; init; }
        public Guid BuyerProfileId { get; init; }
        public Guid SellerProfileId { get; init; }
        public Guid ReportId { get; init; }
        public DateTime DateTimeCreated { get; init; }
        public IEnumerable<OrderItemResponseDto> Items { get; init; } = new List<OrderItemResponseDto>();
    }

    public record OrderBuyerDto
    {
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
        public OrderBuyerDto BuyerProfile { get; init; } = new();
        public IEnumerable<OrderItemResponseDto> Items { get; init; } = new List<OrderItemResponseDto>();
        public IEnumerable<OrderDeliveryLogDto> DeliveryLogs { get; init; } = new List<OrderDeliveryLogDto>();
        public string? DeliveryFeeProofImageUrl { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; init; } = OrderStatus.Pending.ToString();
    }

    public record OrderSellerResponsePaginatedDto
    {
        public IEnumerable<OrderSellerResponseDto> Orders { get; init; } = new List<OrderSellerResponseDto>();
        public int PageCount { get; init; }
    }
}
