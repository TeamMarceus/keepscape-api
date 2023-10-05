using keepscape_api.Models.BaseModels;

namespace keepscape_api.Models
{
    public class CartItem : Base
    {
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public string CustomizationMessage { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public virtual Cart? Cart { get; set; }
        public virtual Product? Product { get; set; }
    }
}