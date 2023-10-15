using keepscape_api.Models;
using keepscape_api.Models.Categories;

namespace keepscape_api.Dtos.Products
{
    public record ProductCategoryPlaceDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Image { get; init; } = string.Empty;
    }

    public record ProductResponseHomeDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public int Stars { get; init; }
        public string Province { get; init; } = string.Empty;
        public string Image { get; init; } = string.Empty;
    }

    public record ProductReviewDto
    {
        public string UserName { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public int Rating { get; init; }
    }

    public record ProductSellerDto
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Phone { get; init; } = string.Empty;
    }
    public record ProductResponseDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public decimal Rating { get; init; }
        public int Quantity { get; init; }
        public bool IsCustomizable { get; init; }
        public bool IsHidden { get; init; }
        public string Province { get; init; } = string.Empty;
        public ProductSellerDto Seller { get; init; } = new ProductSellerDto();
        public IEnumerable<string> Images { get; init; } = new List<string>();
        public IEnumerable<string> Categories { get; init; } = new List<string>();
        public IEnumerable<ProductReviewDto> Reviews { get; init; } = new List<ProductReviewDto>();
    }
}
