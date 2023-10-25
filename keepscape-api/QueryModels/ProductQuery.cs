namespace keepscape_api.QueryModels
{
    public class ProductQuery : PaginatorQuery
    {
        public Guid? SellerProfileId { get; set; }
        public string? Search { get; set; }
        public IEnumerable<string> Provinces { get; set; } = new List<string>();
        public IEnumerable<string> Categories { get; set; } = new List<string>();
        public int? Rating { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool IsHidden { get; set; } = false;
        public bool Descending { get; set; } = false;
    }
}
