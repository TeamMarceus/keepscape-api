using keepscape_api.Enums;
using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class Order : Base
    {
        public Guid BuyerProfileId { get; set; }
        public Guid SellerProfileId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? DeliveryFeeProofImageUrl { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TotalPrice { get; set; }
        public virtual BuyerProfile? BuyerProfile { get; set; }
        public virtual SellerProfile? SellerProfile { get; set; }
        public virtual OrderReport? OrderReport { get; set; }
        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public virtual ICollection<OrderDeliveryLog> DeliveryLogs { get; set; } = new List<OrderDeliveryLog>();
    }
}
