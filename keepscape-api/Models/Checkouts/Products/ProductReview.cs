using keepscape_api.Models.BaseModels;

namespace keepscape_api.Models
{
    public class ProductReview : Base
    {
        public Guid BuyerProfileId { get; set; }
        public Guid ProductId { get; set; }
        public string Review { get; set; } = string.Empty;
        public int Rating { get; set; }
        public virtual BuyerProfile? BuyerProfile { get; set; }
        public virtual Product? Product { get; set; }
    }
}
