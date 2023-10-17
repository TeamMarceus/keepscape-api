using keepscape_api.Dtos.Products;
using keepscape_api.Dtos.Users;
using keepscape_api.Enums;
using keepscape_api.QueryModels;

namespace keepscape_api.Services.Users
{
    public interface IUserService
    {
        Task<UserStatus> GetStatus(string email);
        Task<UserStatus> GetStatus(Guid userId);
        Task<UserSellerApplicationDto?> GetApplication(Guid userId);
        Task<UserSellerApplicationPagedDto> GetApplications(SellerApplicationQuery sellerApplicationQuery);
        Task<bool> UpdateApplication(Guid applicationId, UserSellerApplicationStatusUpdateDto statusUpdate);
        Task<UserResponseBaseDto?> Login(UserLoginDto userLoginDto);
        Task<UserResponseBaseDto?> Register(UserCreateBaseDto userCreateDto);
        Task<UserResponseBaseDto?> Update(Guid userId, UserUpdateBaseDto userUpdateDto);
        Task<bool> Update(Guid userId, UserStatusUpdateDto userStatusUpdateDto);
        Task<bool> UpdatePassword(Guid userId, UserUpdatePasswordDto userUpdatePasswordDto);
        Task<bool> UpdatePasswordWithCode(UserUpdatePasswordWithCodeDto userUpdatePasswordWithCodeDto);
        Task Logout(Guid userId);
    }
}
