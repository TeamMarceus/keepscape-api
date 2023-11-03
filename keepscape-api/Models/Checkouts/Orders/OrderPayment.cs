using keepscape_api.Enums;
using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class OrderPayment : Base
    {
        public Guid OrderId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentMethodOrderId { get; set; } = null!;
        public Order Order { get; set; } = null!;
    }
}
