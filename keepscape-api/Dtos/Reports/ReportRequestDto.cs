using System.ComponentModel.DataAnnotations;

namespace keepscape_api.Dtos.Reports
{
    public record ReportRequestDto
    {
        [Required]
        [StringLength(500)]
        public string Reason { get; init; } = string.Empty;
    }
}
