using System.ComponentModel.DataAnnotations;

namespace keepscape_api.Dtos.Reports
{
    public record ReportProductRequestDto
    {
        [Required]
        public Guid UserId { get; init; }
        [Required]
        public Guid ProductId { get; init; }
        [Required]
        [StringLength(500)]
        public string Reason { get; init; } = string.Empty;
    }

    public record ReportOrderRequestDto
    {
        [Required]
        public Guid UserId { get; init; }
        [Required]
        public Guid OrderId { get; init; }
        [Required]
        [StringLength(500)]
        public string Reason { get; init; } = string.Empty;
    }
}
