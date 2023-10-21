using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class OrderDeliveryLog : Base
    {
        public Guid OrderId { get; set; }
        public DateTime DateTime { get; set; } 
        public string Log { get; set; } = string.Empty;
        public virtual Order? Order { get; set; }
    }
}
