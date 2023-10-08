using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class Token : Base
    {
        public Guid UserId { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
