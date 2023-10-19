using keepscape_api.Enums;

namespace keepscape_api.Dtos.Dashboards.Admin
{
    public record AdminDashboardResponseDto
    {
        public int SellerApplications { get; set; }
        public int OngoingOrders { get; set; }
        public int Products { get; set; }
        public string Year { get; init; } = DateTime.UtcNow.Year.ToString();
        public Dictionary<Month, AdminMonthlyStatsDto> MonthlyStatistics { get; set; } = new();
    }

    public record AdminMonthlyStatsDto
    {
        public int Products { get; set; }
        public int Buyers { get; set; }
        public int Sellers { get; set; }
    }
}
