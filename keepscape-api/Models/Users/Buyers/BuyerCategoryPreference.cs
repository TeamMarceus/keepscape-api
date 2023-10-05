using keepscape_api.Models.BaseModels;

namespace keepscape_api.Models
{
    public class BuyerCategoryPreference : Base
    {
        public Guid BuyerProfileId { get; set; }
        public Guid ProductCategoryId { get; set; }
        public string Description { get; set; } = string.Empty;
        public virtual BuyerProfile? BuyerProfile{ get; set; }
        public virtual ProductCategory? ProductCategory { get; set; }
    }
}
