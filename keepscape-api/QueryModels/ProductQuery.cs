namespace keepscape_api.QueryModels
{
    public class ProductQuery : PaginatorQuery
    {
        public Guid? SellerProfileId { get; set; }
        public string Search { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int? Rating { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool IsHidden { get; set; } = false;
        public bool Descending { get; set; } = false;
    }
}
