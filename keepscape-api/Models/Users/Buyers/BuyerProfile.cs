using keepscape_api.Models.BaseModels;

namespace keepscape_api.Models
{
    public class BuyerProfile : Base
    {
        public Guid UserId { get; set; }
        public Guid CartId { get; set; }
        public string BuyerPreferences { get; set; } = string.Empty;
        public string BuyerInterests { get; set; } = string.Empty;
        public string BuyerDescription { get; set; } = string.Empty;
        public string BuyerDeliveryFullName { get; set; } = string.Empty;
        public string BuyerDeliveryAddress { get; set; } = string.Empty;
        public string BuyerAltMobileNumber { get; set; } = string.Empty;
        public virtual User? User { get; set; }
        public virtual Cart? Cart { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<BuyerCategoryPreference>? BuyerCategoryPreferences { get; set; }
    }
}
