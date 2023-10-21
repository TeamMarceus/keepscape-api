using System.ComponentModel.DataAnnotations;

namespace keepscape_api.Dtos.Reports
{
    public record ReportProductRequestDto
    {
        [Required]
        [StringLength(500)]
        public string Reason { get; init; } = string.Empty;
    }

    public record ReportOrderRequestDto
    {
        [Required]
        [StringLength(500)]
        public string Reason { get; init; } = string.Empty;
    }
}
