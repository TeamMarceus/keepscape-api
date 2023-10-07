using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class TokenRepository : BaseRepository<Token>, ITokenRepository
    {
        public TokenRepository(APIDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Token>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }
        public override async Task<Token?> GetByIdAsync(Guid id)
        {
            return await base.GetByIdAsync(id);
        }
        public async Task<Token?> GetLatestTokenByUserGuid(Guid userId)
        {
            return await _dbSet.Where(t => t.UserId == userId).OrderByDescending(t => t.DateTimeCreated).FirstOrDefaultAsync();
        }
        public async Task<Token?> GetTokenByRefreshToken(string refreshToken)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.RefreshToken == refreshToken);
        }
    }
}
