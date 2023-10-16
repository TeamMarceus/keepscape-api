using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class BalanceHistory : Base
    {
        public Guid BalanceId { get; set; }
        public decimal Amount { get; set; }
        public virtual Balance? Balance { get; set; }
    }
}
