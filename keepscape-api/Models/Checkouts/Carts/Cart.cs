using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class Cart : Base
    {
        public Guid BuyerProfileId { get; set; }
        public virtual BuyerProfile? BuyerProfile{ get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
