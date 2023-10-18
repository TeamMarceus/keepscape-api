using keepscape_api.Enums;
using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class SellerApplication : Base
    {
        public Guid SellerProfileId { get; set; }
        public string IdImageUrl { get; set; } = string.Empty;
        public string BusinessPermitUrl { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        public string? StatusReason { get; set; }
        public virtual SellerProfile SellerProfile { get; set; } = null!;
    }
}
