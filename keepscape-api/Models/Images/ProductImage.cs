using keepscape_api.Models.Primitives;

namespace keepscape_api.Models.Images
{
    public class ProductImage : Base
    {
        public Guid ProductId { get; set; }
        public Guid BaseImageId { get; set; }
        public virtual BaseImage? BaseImage { get; set; }
        public virtual Product? Product { get; set; }
    }
}
