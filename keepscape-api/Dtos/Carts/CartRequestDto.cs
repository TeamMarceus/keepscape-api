using System.ComponentModel.DataAnnotations;

namespace keepscape_api.Dtos.Carts
{
    public record CartRequestDto
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        [Range(1, 1000000, ErrorMessage = $"{nameof(Quantity)} must be between 1 and 1000000")]
        public int Quantity { get; set; }
        public string? CustomizationMessage { get; set; }
    }

    public record CartUpdateDto
    {
        [Required]
        public IDictionary<Guid, CartItemUpdateDto> CartItems { get; init; } = new Dictionary<Guid, CartItemUpdateDto>();
    }

    public record CartItemUpdateDto
    {
        public int? Quantity { get; init; }
        public string? CustomizationMessage { get; init; }
    }
}
