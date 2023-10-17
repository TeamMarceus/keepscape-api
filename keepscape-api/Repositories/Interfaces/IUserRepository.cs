using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByPhoneNumberAsync(string phoneNumber);
        Task<(IEnumerable<User> Buyers, int PageCount)> GetBuyers(PaginatorQuery paginatorQuery);
        Task<(IEnumerable<User> Sellers, int PageCount)> GetSellers(PaginatorQuery paginatorQuery);
        Task<int> GetUserCountAsync();
    }
}
