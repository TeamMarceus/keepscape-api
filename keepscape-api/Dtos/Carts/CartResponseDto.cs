namespace keepscape_api.Dtos.Carts
{
    public record CartResponseDto
    {
        public Guid Id { get; set; }
        public IDictionary<string, CartItemResponseDto> SellerCartItems { get; set; } = new Dictionary<string, CartItemResponseDto>();
    }

    public record CartItemResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string CustomizationMessage { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string ProductImageUrl { get; set; } = string.Empty;
    }
}
