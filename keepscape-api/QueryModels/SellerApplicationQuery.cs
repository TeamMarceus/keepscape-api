namespace keepscape_api.QueryModels
{
    public class SellerApplicationQuery : PaginatorQuery
    {
        public string? Search { get; set; }
        public string? Status { get; set; }
        public string? OrderBy { get; set; }
        public bool IsDescending { get; set; } = false;
    }
}
