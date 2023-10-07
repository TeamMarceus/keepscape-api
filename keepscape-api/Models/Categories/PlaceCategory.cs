using keepscape_api.Models.Categories;

namespace keepscape_api.Models
{
    public class PlaceCategory : BaseCategory
    {
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
