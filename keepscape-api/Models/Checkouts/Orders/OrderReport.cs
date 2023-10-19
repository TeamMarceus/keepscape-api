using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class OrderReport : Base
    {
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
        public string? Reason { get; set; } 
        public bool IsResolved { get; set; } = false;
        public bool IsRefunded { get; set; } = false;
        public virtual User User { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
    }
}
