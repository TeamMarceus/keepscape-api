using keepscape_api.Enums;
using keepscape_api.Models.Primitives;

namespace keepscape_api.Models
{
    public class User : Base
    {
        public UserType UserType { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsBanned { get; set; } = false;
        public bool IsVerified { get; set; } = false;
        public DateTime LastLoggedIn { get; set; }
        public virtual BuyerProfile? BuyerProfile { get; set; }
        public virtual SellerProfile? SellerProfile { get; set; }
        public virtual Balance? Balance { get; set; }
        public virtual ICollection<ConfirmationCode>? ConfirmationCodes { get; set; }
        public virtual ICollection<Token>? Tokens { get; set; }
    }
}
