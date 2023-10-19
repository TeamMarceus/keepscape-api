using keepscape_api.Enums;
using keepscape_api.Models.Primitives;

namespace keepscape_api.Models.Users.Finances
{
    public class BalanceWithdrawal : Base
    {
        public Guid SellerProfileId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentDetails { get; set; } = string.Empty;
        public string PaymentProfileImageUrl { get; set; } = string.Empty;
        public string? PaymentProofImageUrl { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public virtual SellerProfile? SellerProfile { get; set; }
    }
}
