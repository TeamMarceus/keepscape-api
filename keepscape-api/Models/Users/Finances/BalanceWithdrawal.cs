using keepscape_api.Enums;
using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class BalanceWithdrawal : Base
    {
        public Guid BalanceId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentDetails { get; set; } = string.Empty;
        public string PaymentProfileImageUrl { get; set; } = string.Empty;
        public string? PaymentProofImageUrl { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public virtual Balance Balance { get; set; } = null!;
    }
}
