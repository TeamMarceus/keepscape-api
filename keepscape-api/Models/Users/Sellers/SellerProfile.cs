using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class SellerProfile : Base, IProfile
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public virtual User? User { get; set; }
        public virtual SellerApplication? SellerApplication { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
    }
}
