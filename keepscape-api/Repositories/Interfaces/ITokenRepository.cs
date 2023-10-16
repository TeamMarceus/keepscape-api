using keepscape_api.Models;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface ITokenRepository : IBaseRepository<Token>
    {
        Task<Token?> GetLatestTokenByUserGuid(Guid userId);
        Task<Token?> GetTokenByRefreshToken(string refreshToken);
    }
}
