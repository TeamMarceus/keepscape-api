using keepscape_api.Models.Categories;

namespace keepscape_api.Models
{
    public class ProductCategory : BaseCategory
    {
        public string ProductCategoryName { get; set; } = string.Empty;
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
