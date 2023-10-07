using System.ComponentModel.DataAnnotations;

namespace keepscape_api.Dtos.Tokens
{
    public record TokenVerifyDto
    {
        [Required]
        public Guid UserId { get; init; }
        [Required]
        public string AccessToken { get; init; } = string.Empty;
        [Required]
        public string RefreshToken { get; init; } = string.Empty;
    }

    public record TokenCreateDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; } = string.Empty;

        [Required]
        public string Password { get; init; } = string.Empty;
    }

}
