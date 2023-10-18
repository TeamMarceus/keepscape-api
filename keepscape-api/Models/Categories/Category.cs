using keepscape_api.Models.Primitives;

namespace keepscape_api.Models.Categories
{
    public class Category : Base
    {
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
