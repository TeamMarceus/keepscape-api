namespace keepscape_api.Dtos.Products
{
    public record ProductCategoryDto
    {
        public string Name { get; init; } = null!;
        public string Image { get; init; } = null!;
    }
    public class ProductCategoryListDto
    {
        public string Category { get; init; } = null!;
        public List<ProductCategoryDto> Subcategories { get; init; } = new List<ProductCategoryDto>();
    }

    public class ProductResponseDto
    {
        public Guid Id { get; init; }
        public decimal Price { get; init; }
        public int Rating { get; init; }
        public string Province { get; init; } = null!;
    }
}
