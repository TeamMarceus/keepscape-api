using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class BaseImage : Base
    {
        public string? Url { get; set; } = string.Empty;
        public string? Alt { get; set; } = string.Empty;
    }
}
