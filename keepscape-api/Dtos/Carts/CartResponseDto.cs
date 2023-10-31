namespace keepscape_api.Dtos.Carts
{
    public record CartResponseDto
    {
        public Guid Id { get; set; }
        public IEnumerable<CartSellerDto> CartSellers { get; set; } = new List<CartSellerDto>();
        public IEnumerable<CartItemResponseDto> HiddenItems { get; set; } = new List<CartItemResponseDto>();
        public string? Message { get; set; }
    }

    public record CartSellerDto
    {
        public Guid Id { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public IEnumerable<CartItemResponseDto> CartItems { get; set; } = new List<CartItemResponseDto>();
    }

    public record CartItemResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsCustomizable { get; set; }
        public string CustomizationMessage { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string ProductImageUrl { get; set; } = string.Empty;
    }
}
