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

    public record OrderResponseAdminDto
    {
        public Guid Id { get; init; }
        public Guid BuyerProfileId { get; init; }
        public Guid SellerProfileId { get; init; }
        public Guid ReportId { get; init; }
        public DateTime DateTimeCreated { get; init; }
        public IEnumerable<OrderItemResponseDto> Items { get; init; } = new List<OrderItemResponseDto>();

    }
}
