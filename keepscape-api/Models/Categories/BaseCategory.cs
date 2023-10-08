using keepscape_api.Enums;
using keepscape_api.Models.Primitives;

namespace keepscape_api.Models.Categories
{
    public class BaseCategory : Base
    {
        public Guid? BaseImageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public CategoryType Type { get; set; }
        public virtual BaseImage? BaseImage { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
