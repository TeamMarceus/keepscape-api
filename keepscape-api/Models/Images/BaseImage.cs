using keepscape_api.Models.BaseModels;

namespace keepscape_api.Models
{
    public class BaseImage : Base
    {
        public string? Url { get; set; } = string.Empty;
        public string? Alt { get; set; } = string.Empty;
    }
}
