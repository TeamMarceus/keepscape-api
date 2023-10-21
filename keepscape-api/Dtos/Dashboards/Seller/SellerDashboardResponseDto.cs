namespace keepscape_api.Dtos.Dashboards.Seller
{
    public record SellerDashboardResponseDto
    {
        public int PendingOrders { get; set; }
        public int OngoingOrders { get; set; }
        public int CompletedOrders { get; set; }
    }
}
