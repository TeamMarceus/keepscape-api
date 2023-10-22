using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class Balance : Base
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<BalanceWithdrawal> Withdrawals { get; set; } = new List<BalanceWithdrawal>();
        public virtual ICollection<BalanceLog> Histories { get; set; } = new List<BalanceLog>();
    }
}
