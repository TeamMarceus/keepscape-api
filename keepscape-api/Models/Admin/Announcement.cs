using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class Announcement : Base
    {
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
    }
}
