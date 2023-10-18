namespace keepscape_api.QueryModels
{
    public class SellerApplicationQuery : PaginatorQuery
    {
        public string Status { get; set; } = string.Empty;
        public string OrderBy { get; set; } = string.Empty;
        public bool IsDescending { get; set; } = false;
    }
}
