namespace keepscape_api.QueryModels
{
    public class OrderReportQuery : PaginatorQuery
    {
        public string? SellerName { get; set; }
        public string? BuyerName { get; set; }
    }
}
