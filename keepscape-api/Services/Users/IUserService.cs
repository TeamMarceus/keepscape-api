using keepscape_api.Dtos.Users;
using keepscape_api.Enums;

namespace keepscape_api.Services.Users
{
    public interface IUserService
    {
        Task<UserStatus> GetStatus(string email);
        Task<UserStatus> GetStatus(Guid userId);
        Task<UserResponseBaseDto?> Login(UserLoginDto userLoginDto);
        Task<UserResponseBaseDto?> Register(UserCreateBaseDto userCreateDto);
        Task<UserResponseBaseDto?> Update(Guid userId, UserUpdateBaseDto userUpdateDto);
        Task<bool> UpdatePassword(Guid userId, UserUpdatePasswordDto userUpdatePasswordDto);
        Task<bool> UpdatePasswordWithCode(UserUpdatePasswordWithCodeDto userUpdatePasswordWithCodeDto);
        Task Logout(Guid userId);
    }
}
