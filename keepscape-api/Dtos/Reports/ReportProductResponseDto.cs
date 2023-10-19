namespace keepscape_api.Dtos.Reports
{
    public record ReportProductResponseDto
    {
        public DateTime DateTimeCreated { get; init; }
        public Guid UserId { get; init; }
        public string? Reason { get; init; } = string.Empty;
    }

    public record ReportOrderResponseDto
    {
        public Guid UserId { get; init; }
        public Guid OrderId { get; init; }
        public Guid SellerId { get; init; }
        public string? Reason { get; init; } = string.Empty;
    }
}
