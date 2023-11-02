using keepscape_api.Dtos.Users;
using keepscape_api.Enums;
using keepscape_api.QueryModels;

namespace keepscape_api.Services.Users
{
    public interface IUserService
    {
        Task<UserStatusDto> GetStatus(string email);
        Task<UserStatusDto> GetStatus(Guid userId);
        Task<(UserResponseBaseDto? user, UserType? type)> GetById(Guid userId);
        Task<UserSellerApplicationDto?> GetApplication(Guid userId);
        Task<UserSellerApplicationPagedDto> GetApplications(SellerApplicationQuery sellerApplicationQuery);
        Task<UserBuyersPagedDto> GetBuyers(UserQuery userQuery);
        Task<UserSellersPagedDto> GetSellers(UserQuery userQuery);
        Task<bool> UpdateApplication(Guid applicationId, UserSellerApplicationStatusUpdateDto statusUpdate);
        Task<UserResponseBaseDto?> Login(UserLoginDto userLoginDto);
        Task<UserResponseBaseDto?> Register(UserCreateBaseDto userCreateDto);
        Task<UserResponseBaseDto?> Update(Guid userId, UserUpdateBaseDto userUpdateDto);
        Task<bool> CreateBuyerSuggestions(Guid userId);
        Task<IEnumerable<UserBuyerSuggestionsDto>> GetBuyerSuggestions(Guid userId);
        Task<bool> Update(Guid userId, UserStatusUpdateDto userStatusUpdateDto);
        Task<bool> UpdatePassword(Guid userId, UserUpdatePasswordDto userUpdatePasswordDto);
        Task<bool> UpdatePasswordWithCode(UserUpdatePasswordWithCodeDto userUpdatePasswordWithCodeDto);
        Task Logout(Guid userId);
    }
}
