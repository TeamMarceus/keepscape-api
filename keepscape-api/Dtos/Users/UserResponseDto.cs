using keepscape_api.Enums;

namespace keepscape_api.Dtos.Users
{
    public abstract record UserResponseBaseDto
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public bool IsBanned { get; init; } = false;
        public DateTime DateTimeCreated { get; init; }
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
        public string IdImageUrl { get; init; } = string.Empty;
        public string BusinessPermitUrl { get; init; } = string.Empty;
        public string SellerName { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string UserType { get; init; } = Enums.UserType.Seller.ToString();
        public DateTime DateTimeApproved { get; init; }
    }

    public record UserResponseAdminDto : UserResponseBaseDto
    {
        public string UserType { get; init; } = Enums.UserType.Admin.ToString();
    }

    public record UserSellerApplicationDto 
    { 
        public Guid Id { get; init; }
        public string Status { get; init; } = ApplicationStatus.Pending.ToString();
        public string IdImageUrl { get; init; } = string.Empty;
        public string BusinessPermitUrl { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string SellerName { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public DateTime DateTimeCreated { get; init; }
    }

    public record UserSellerApplicationPagedDto
    {
        public IEnumerable<UserSellerApplicationDto> SellerApplications { get; init; } = new List<UserSellerApplicationDto>();
        public int PageCount { get; init; } = 1;
    }

    public record UserBuyersPagedDto
    {
        public IEnumerable<UserResponseBuyerDto> Buyers { get; init; } = new List<UserResponseBuyerDto>();
        public int PageCount { get; init; } = 1;
    }

    public record UserSellersPagedDto
    {
        public IEnumerable<UserResponseSellerDto> Sellers { get; init; } = new List<UserResponseSellerDto>();
        public int PageCount { get; init; } = 1;
    }
}
