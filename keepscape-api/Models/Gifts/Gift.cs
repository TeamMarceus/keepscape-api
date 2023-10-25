using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class Gift : Base
    {
        public Guid OrderItemId { get; set; }
        public Guid UserId { get; set; }
        public string? Message { get; set; } = string.Empty;
    }
}
