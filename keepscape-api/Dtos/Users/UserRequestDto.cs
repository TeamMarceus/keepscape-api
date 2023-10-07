using keepscape_api.Validators;
using System.ComponentModel.DataAnnotations;

namespace keepscape_api.Dtos.Users
{

    public record UserLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; } = string.Empty;
        [Required]
        public string Password { get; init; } = string.Empty;
    }
    public abstract record UserCreateBaseDto 
    {
        [Required]
        [MinLength(2, ErrorMessage = $"{nameof(FirstName)} can have at least 2 characters")]
        [MaxLength(50, ErrorMessage = $"{nameof(FirstName)} can have at most 50 characters")]
        public string FirstName { get; init; } = string.Empty;
        [Required]
        [MinLength(2, ErrorMessage = $"{nameof(LastName)} can have at least 2 characters")]
        [MaxLength(50, ErrorMessage = $"{nameof(LastName)} can have at most 50 characters")]
        public string LastName { get; init; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; init; } = string.Empty;
        [Required]
        [MinLength(6, ErrorMessage = $"{nameof(Password)} too short")]
        [MaxLength(20, ErrorMessage = $"{nameof(Password)} can have at most 20 characters")]
        public string Password { get; init; } = string.Empty;
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; init; } = string.Empty;
        [Required]
        [Phone]
        public string PhoneNumber { get; init; } = string.Empty;
    }

    public record UserCreateBuyerDto : UserCreateBaseDto
    {
        [Required]
        public string Preferences { get; init; } = string.Empty;
        [Required]
        public string Interests { get; init; } = string.Empty;
        [Required]
        public string Description { get; init; } = string.Empty;
    }

    public record UserCreateSellerDto : UserCreateBaseDto
    {
        [Required]
        public IFormFile BaseImage { get; init; } = null!;
        [Required]
        public string SellerName { get; init; } = string.Empty;
        [Required]
        public string Description { get; init; } = string.Empty;
    }

    public record UserUpdateBaseDto;
    public record UserUpdateBuyerDto : UserUpdateBaseDto
    {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? Preferences { get; init; }
        public string? Interests { get; init; }
        public string? Description { get; init; }
        public string? DeliveryFullName { get; init; }
        public string? DeliveryAddress { get; init; }
        public string? AltMobileNumber { get; init; }
    }
    public record UserUpdateSellerDto : UserUpdateBaseDto
    {
        public string? SellerName { get; init; }
        public string? Description { get; init; }
    }
    public record UserUpdatePasswordDto
    {
        [Required]
        [MinLength(6, ErrorMessage = $"{nameof(OldPassword)} too short")]
        [MaxLength(20, ErrorMessage = $"{nameof(OldPassword)} can have at most 20 characters")]
        public string OldPassword { get; init; } = string.Empty;
        [Required]
        [MinLength(6, ErrorMessage = $"{nameof(NewPassword)} too short")]
        [MaxLength(20, ErrorMessage = $"{nameof(NewPassword)} can have at most 20 characters")]
        [NotEqualTo(nameof(OldPassword), ErrorMessage = "New password must be different from old password")]
        public string NewPassword { get; init; } = string.Empty;
        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; init; } = string.Empty;
    }

    public record class UserUpdatePasswordWithCodeDto : UserUpdatePasswordDto
    {
        [Required]
        public string Code { get; init; } = string.Empty;
    }
}
