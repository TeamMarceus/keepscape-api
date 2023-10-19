using keepscape_api.Models.Primitives;

namespace keepscape_api.Models.Checkouts.Orders
{
    public class OrderReport : Base
    {
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
        public string? Reason { get; set; } 
        public bool IsResolved { get; set; } = false;
        public virtual Order Order { get; set; } = null!;
    }
}
