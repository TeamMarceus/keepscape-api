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
        [Range(0, 1000000, ErrorMessage = $"{nameof(BuyerPrice)} must be between 0 and 1000000")]
        public decimal BuyerPrice { get; init; }
        [Required]
        [Range(0, 1000000, ErrorMessage = $"{nameof(SellerPrice)} must be between 0 and 1000000")]
        public decimal SellerPrice { get; init; }
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
        public IFormFile Image1 { get; init; } = null!;
        public IFormFile? Image2 { get; init; } 
        public IFormFile? Image3 { get; init; }
        public IFormFile? Image4 { get; init; }
        public IFormFile? Image5 { get; init; }
    }

    public record ProductReviewCreateDto
    {
        public string? Review { get; init; }
        [Required]
        [Range(1, 5, ErrorMessage = $"{nameof(Rating)} must be between 1 and 5")]
        public int Rating { get; init; }
    }

    public record ProductReviewUpdateDto
    {
        public string? Review { get; init; }
        [Range(1, 5, ErrorMessage = $"{nameof(Rating)} must be between 1 and 5")]
        public int? Rating { get; init; }
    }

    public record ProductUpdateDto
    {
        [MinLength(2, ErrorMessage = $"{nameof(Name)} can have at least 2 characters")]
        [MaxLength(50, ErrorMessage = $"{nameof(Name)} can have at most 50 characters")]
        public string? Name { get; init; }
        [MinLength(2, ErrorMessage = $"{nameof(Description)} can have at least 2 characters")]
        [MaxLength(500, ErrorMessage = $"{nameof(Description)} can have at most 500 characters")]
        public string? Description { get; init; }
        [Range(0, 1000000, ErrorMessage = $"{nameof(BuyerPrice)} must be between 0 and 1000000")]
        public decimal? BuyerPrice { get; init; }
        [Range(0, 1000000, ErrorMessage = $"{nameof(SellerPrice)} must be between 0 and 1000000")]
        public decimal? SellerPrice { get; init; }
        [Range(0, 1000000, ErrorMessage = $"{nameof(Quantity)} must be between 0 and 1000000")]
        public int? Quantity { get; init; }
        public bool? IsCustomizable { get; init; }
        public bool? IsHidden { get; init; }
        public Guid? PlaceId { get; init; }
        public IEnumerable<Guid>? CategoryIds { get; init; }
        public IDictionary<string, IFormFile>? Images { get; init; }
        public IFormFile? Image1 { get; init; }
        public IFormFile? Image2 { get; init; }
        public IFormFile? Image3 { get; init; }
        public IFormFile? Image4 { get; init; }
        public IFormFile? Image5 { get; init; }
    }

    public record ProductCategoryPlaceCreateDto
    {
        [Required]
        [MinLength(2, ErrorMessage = $"{nameof(Name)} can have at least 2 characters")]
        [MaxLength(50, ErrorMessage = $"{nameof(Name)} can have at most 50 characters")]
        public string Name { get; init; } = string.Empty;
        [Required]
        public IFormFile Image { get; init; } = null!;
    }
}
