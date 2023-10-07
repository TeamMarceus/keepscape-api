namespace keepscape_api.QueryModels
{
    public class ProductQueryParameters
    {
        public string Search { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public Guid? SellerId { get; set; }
        public int? Rating { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public bool Descending { get; set; } = false;
    }
}
