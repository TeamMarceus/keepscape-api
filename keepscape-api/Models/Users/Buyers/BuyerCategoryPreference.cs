using keepscape_api.Models.Categories;
using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class BuyerCategoryPreference : Base
    {
        public Guid BuyerProfileId { get; set; }
        public Guid ProductCategoryId { get; set; }
        public string Description { get; set; } = string.Empty;
        public virtual BuyerProfile? BuyerProfile{ get; set; }
        public virtual Category? Category { get; set; }
    }
}
