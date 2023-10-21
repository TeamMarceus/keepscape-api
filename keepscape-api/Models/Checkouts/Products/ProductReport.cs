using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class ProductReport : Base
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public string? Reason { get; set; } = string.Empty;
        public bool IsResolved { get; set; } = false;
        public virtual User User { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
