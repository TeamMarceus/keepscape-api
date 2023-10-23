namespace keepscape_api.Dtos.Reports
{
    public abstract record ReportResponseDto
    {
        public DateTime DateTimeCreated { get; init; }
        public Guid UserId { get; init; }
        public string? Reason { get; init; } = string.Empty;
    }
    public record ReportProductResponseDto : ReportResponseDto
    {
        public string BuyerName { get; init; } = string.Empty;  
    }

    public record ReportOrderResponseDto : ReportResponseDto
    {
    }
}
