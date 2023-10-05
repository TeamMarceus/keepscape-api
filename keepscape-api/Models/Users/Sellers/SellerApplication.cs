using keepscape_api.Enums;
using keepscape_api.Models.BaseModels;

namespace keepscape_api.Models
{
    public class SellerApplication : Base
    {
        public Guid SellerProfileId { get; set; }
        public Guid BaseImageId { get; set; }
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        public virtual BaseImage? BaseImage { get; set; }
        public virtual SellerProfile? SellerProfile{ get; set; }
    }
}
