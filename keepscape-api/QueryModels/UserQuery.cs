namespace keepscape_api.QueryModels
{
    public class UserQuery : PaginatorQuery
    {
        public bool? IsBanned { get; set; }
        public string? Search { get; set; } = string.Empty;
        public string? OrderBy { get; set; } = string.Empty;
        public bool IsDescending { get; set; } = false;
    }
}
