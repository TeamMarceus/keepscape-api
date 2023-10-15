using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class ConfirmationCode : Base
    {
        public Guid UserId { get; set; }
        public string Code { get; set; } = string.Empty;
        public bool IsConfirmed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime ExpiresAt { get; set; }
        public virtual User? User { get; set; }
    }
}
