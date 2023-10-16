using keepscape_api.Models.Primitives;

namespace keepscape_api.Models.Checkouts.Products
{
    public class ProductReport : Base
    {
        public Guid UserGuid { get; set; }
        public Guid ProductGuid { get; set; }
        public string? Reason { get; set; } = string.Empty;
        public virtual User User { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
