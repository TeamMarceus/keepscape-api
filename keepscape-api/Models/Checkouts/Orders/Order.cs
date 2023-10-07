using keepscape_api.Enums;
using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class Order : Base
    {
        public Guid BuyerProfileId { get; set; }
        public Guid ProductId { get; set; }
        public Guid BaseImageId { get; set; }
        public int Quantity { get; set; }
        public string? CustomizedMessage { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
        public virtual BaseImage? BaseImage { get; set; }
        public virtual BuyerProfile? BuyerProfile { get; set; }
        public virtual Product? Product { get; set; }
        public virtual ICollection<OrderDeliveryLog> OrderDeliveryLogs { get; set; } = new List<OrderDeliveryLog>();
    }
}
