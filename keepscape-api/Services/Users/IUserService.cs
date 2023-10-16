using keepscape_api.Dtos.Users;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.QueryModels;

namespace keepscape_api.Services.Users
{
    public interface IUserService
    {
        Task<UserStatus> GetStatus(string email);
        Task<UserStatus> GetStatus(Guid userId);
        Task<SellerApplication?> GetApplication(Guid userId);
        Task<IEnumerable<SellerApplication>> GetApplications(PaginatorQuery paginatorQuery);
        Task<bool> UpdateApplication(Guid applicationId, UserSellerApplicationStatusUpdateDto statusUpdate);
        Task<UserResponseBaseDto?> Login(UserLoginDto userLoginDto);
        Task<UserResponseBaseDto?> Register(UserCreateBaseDto userCreateDto);
        Task<UserResponseBaseDto?> Update(Guid userId, UserUpdateBaseDto userUpdateDto);
        Task<bool> UpdatePassword(Guid userId, UserUpdatePasswordDto userUpdatePasswordDto);
        Task<bool> UpdatePasswordWithCode(UserUpdatePasswordWithCodeDto userUpdatePasswordWithCodeDto);
        Task Logout(Guid userId);
    }
}
