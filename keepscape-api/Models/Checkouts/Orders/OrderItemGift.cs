using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class OrderItemGift : Base
    {
        public Guid OrderItemId { get; set; }
        public Guid UserId { get; set; }
        public Guid PlaceId { get; set; }
        public string? Message { get; set; } = string.Empty;
        public virtual OrderItem? OrderItem { get; set; }
        public virtual User? User { get; set; }
        public virtual Place? Place { get; set; }
    }
}
