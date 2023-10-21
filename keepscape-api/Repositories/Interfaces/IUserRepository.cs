using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByPhoneNumberAsync(string phoneNumber);
        Task<(IEnumerable<User> Buyers, int PageCount)> GetBuyers(UserQuery userQuery);
        Task<(IEnumerable<User> Sellers, int PageCount)> GetSellers(UserQuery userQuery);
        Task<int> GetUserCountAsync();
    }
}
