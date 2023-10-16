using System.ComponentModel.DataAnnotations;

namespace keepscape_api.Dtos.Products
{
    public record ProductCreateDto
    {
        [Required]
        [MinLength(2, ErrorMessage = $"{nameof(Name)} can have at least 2 characters")]
        [MaxLength(50, ErrorMessage = $"{nameof(Name)} can have at most 50 characters")]
        public string Name { get; init; } = string.Empty;
        [Required]
        [MinLength(2, ErrorMessage = $"{nameof(Description)} can have at least 2 characters")]
        [MaxLength(500, ErrorMessage = $"{nameof(Description)} can have at most 500 characters")]
        public string Description { get; init; } = string.Empty;
        [Required]
        [Range(0, 1000000, ErrorMessage = $"{nameof(Price)} must be between 0 and 1000000")]
        public decimal Price { get; init; }
        [Required]
        [Range(0, 1000000, ErrorMessage = $"{nameof(Quantity)} must be between 0 and 1000000")]
        public int Quantity { get; init; }
        [Required]
        public Guid PlaceId { get; init; }
        [Required]
        public bool IsCustomizable { get; init; }
        [Required]
        [MinLength(1, ErrorMessage = $"{nameof(CategoryIds)} must have at least 1 category")]
        public IEnumerable<Guid> CategoryIds { get; init; } = new List<Guid>();
        [Required]
        [MinLength(1, ErrorMessage = $"{nameof(ProductImages)} must have at least 1 image")]
        [MaxLength(5, ErrorMessage = $"{nameof(ProductImages)} can have at most 5 images")]
        public IEnumerable<IFormFile> ProductImages { get; init; } = new List<IFormFile>();
    }

    public record ProductReviewCreateDto
    {
        public string? Review { get; init; }
        public int Rating { get; init; }
    }

    public record ProductReviewUpdateDto
    {
        public string? Review { get; init; }
        public int? Rating { get; init; }
    }

    public record ProductUpdateDto
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
        public decimal? Price { get; init; }
        public int? Quantity { get; init; }
        public bool? IsCustomizable { get; init; }
        public bool? IsHidden { get; init; }
        public IEnumerable<Guid>? CategoryIds { get; init; }
        public IEnumerable<IFormFile>? Images { get; init; }
    }
}
