using keepscape_api.Models.Categories;

namespace keepscape_api.Models
{
    public class PlaceCategory : BaseCategory
    {
        public string RegionName { get; set; } = string.Empty;
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
