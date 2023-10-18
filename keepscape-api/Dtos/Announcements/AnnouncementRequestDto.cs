using System.ComponentModel.DataAnnotations;

namespace keepscape_api.Dtos.Announcements
{
    public record AnnouncementRequestDto
    {
        [Required]
        [MinLength(2, ErrorMessage = $"{nameof(Title)} can have at least 2 characters")]
        public string Title { get; init; } = string.Empty;
        [Required]
        [MinLength(2, ErrorMessage = $"{nameof(Description)} can have at least 2 characters")]
        public string Description { get; init; } = string.Empty;
    }
}
