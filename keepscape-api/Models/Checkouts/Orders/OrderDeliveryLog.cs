using keepscape_api.Models.BaseModels;

namespace keepscape_api.Models
{
    public class OrderDeliveryLog : Base
    {
        public Guid OrderId { get; set; }
        public string Log { get; set; } = string.Empty;
        public virtual Order? Order { get; set; }
    }
}
