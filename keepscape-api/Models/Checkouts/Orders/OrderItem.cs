using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class OrderItem : Base
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string? CustomizationMessage { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? QRImageUrl { get; set; }
        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
    }
}
