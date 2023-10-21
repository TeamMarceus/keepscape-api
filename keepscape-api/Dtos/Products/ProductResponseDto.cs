namespace keepscape_api.Dtos.Products
{
    public record ProductCategoryPlaceDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string ImageUrl { get; init; } = string.Empty;
    }

    public record ProductCategoryPlaceNoImageDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }

    public record ProductResponseHomeDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public int Stars { get; init; }
        public ProductCategoryPlaceNoImageDto Province { get; init; } = new ProductCategoryPlaceNoImageDto();
        public string ImageUrl { get; init; } = string.Empty;
    }

    public record ProductResponseHomePaginatedDto
    {
        public IEnumerable<ProductResponseHomeDto> Products { get; init; } = new List<ProductResponseHomeDto>();

        public int PageCount { get; init; } = 1;
    }

    public record ProductResponseAdminDto
    {
        public Guid Id { get; init; }
        public Guid SellerUserId { get; init; }
        public DateTime DateTimeCreated { get; init; }
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public int Quantity { get; init; }
        public IEnumerable<string> ImageUrls { get; init; } = new List<string>();
        public int TotalSold { get; init; }
        public int TotalReports { get; init; }
    }

    public record ProductResponseAdminPaginatedDto
    {
        public IEnumerable<ProductResponseAdminDto> Products { get; init; } = new List<ProductResponseAdminDto>();
        public int PageCount { get; init; } = 1;
    }

    public record ProductReviewDto
    {
        public string UserName { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public int Rating { get; init; }
    }

    public record ProductReviewResponseDto
    {
        public Guid Id { get; init; }
        public string Description { get; init; } = string.Empty;
        public int Rating { get; init; }
    }

    public record ProductReviewPaginatedDto
    {
        public IEnumerable<ProductReviewResponseDto> Reviews { get; init; } = new List<ProductReviewResponseDto>();
        public int PageCount { get; init; } = 1;
    }
    public record ProductSellerDto
    {
        public Guid SellerProfileId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Phone { get; init; } = string.Empty;
        public int Stars { get; init; }
    }
    public record ProductResponseDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public int Stars { get; init; }
        public decimal Price { get; init; }
        public int Quantity { get; init; }
        public bool IsCustomizable { get; init; }
        public bool IsHidden { get; init; }
        public int TotalSold { get; set; }
        public int TotalRatings { get; set; }
        public ProductSellerDto Seller { get; init; } = new ProductSellerDto();
        public IEnumerable<string> Images { get; init; } = new List<string>();
    }
}
