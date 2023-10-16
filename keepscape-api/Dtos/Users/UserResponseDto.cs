using keepscape_api.Enums;

namespace keepscape_api.Dtos.Users
{
    public abstract record UserResponseBaseDto
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
    }

    public record UserResponseBuyerDto : UserResponseBaseDto
    {
        public Guid BuyerProfileId { get; init; }
        public string Preferences { get; init; } = string.Empty;
        public string Interests { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string DeliveryFullName { get; init; } = string.Empty;
        public string DeliveryAddress { get; init; } = string.Empty;
        public string AltMobileNumber { get; init; } = string.Empty;
        public string UserType { get; init; } = Enums.UserType.Buyer.ToString();
    }

    public record UserResponseSellerDto : UserResponseBaseDto
    {
        public Guid SellerProfileId { get; init; }
        public Guid SellerApplicationId { get; init; }
        public string SellerName { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string UserType { get; init; } = Enums.UserType.Seller.ToString();
    }

    public record UserResponseAdminDto : UserResponseBaseDto
    {
        public string UserType { get; init; } = Enums.UserType.Admin.ToString();
    }

    public record UserSellerApplicationStatusUpdateDto
    {
        public string Status { get; init; } = ApplicationStatus.Pending.ToString();
    }

    public record UserStatusUpdateDto
    {
        public string Status { get; init; } = UserStatus.OK.ToString();
    }
}
