using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class BuyerProfile : Base, IProfile
    {
        public Guid UserId { get; set; }
        public Guid CartId { get; set; }
        public string Preferences { get; set; } = string.Empty;
        public string Interests { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DeliveryFullName { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string AltMobileNumber { get; set; } = string.Empty;
        public virtual User? User { get; set; }
        public virtual Cart? Cart { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<BuyerCategoryPreference>? BuyerCategoryPreferences { get; set; }
    }
}
