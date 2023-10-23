namespace keepscape_api.QueryModels
{
    public class ProductReportQuery : PaginatorQuery
    {
        public string? ProductName { get; set; }
        public string? SellerName { get; set; }
    }
}
