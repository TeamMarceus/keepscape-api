using keepscape_api.Models.Primitives;

namespace keepscape_api.Models.Checkouts.Products
{
    public class ProductImage : Base
    {
        public Guid ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public virtual Product Product { get; set; } = null!;
    }
}
