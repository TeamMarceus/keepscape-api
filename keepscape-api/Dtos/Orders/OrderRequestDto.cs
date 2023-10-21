using System.ComponentModel.DataAnnotations;

namespace keepscape_api.Dtos.Orders
{
    public record OrderRequestDto
    {
    }

    public record OrderUpdateDeliveryFeeDto
    {
        [Required]
        [Range(0, 1000000)]
        public decimal DeliveryFee { get; init; }
        [Required]
        public IFormFile DeliveryFeeProofImage { get; init; } = null!;
    }

    public record OrderAddDeliveryLogDto
    {
        [Required]
        [MaxLength(1000)]
        public string Log { get; init; } = null!;
        [Required]
        public DateTime DateTime { get; init; }
    }
}
