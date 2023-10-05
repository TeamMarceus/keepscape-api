using keepscape_api.Models.BaseModels;

namespace keepscape_api.Models.Categories
{
    public class BaseCategory : Base
    {
        public Guid BaseImageId { get; set; }
        public virtual BaseImage? BaseImage { get; set; }
    }
}
