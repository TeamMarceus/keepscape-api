namespace keepscape_api.QueryModels
{
    public class OrderQuery : PaginatorQuery
    {
        public Guid? SellerProfileId { get; set; }
        public Guid? BuyerProfileId { get; set; }
        public string? Status { get; set; }
        public string? ProductName { get; set; }
        public string? BuyerName { get; set; }
        public string? SellerName { get; set; }
    }
}
