namespace keepscape_api.QueryModels
{
    public class BalanceWithdrawalQuery : PaginatorQuery
    {
        public string? PaymentStatus { get; set; }
        public string? SellerName { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
