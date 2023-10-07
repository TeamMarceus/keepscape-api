using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class ConfirmationCodeRepository : BaseRepository<ConfirmationCode>, IConfirmationCodeRepository
    {
        public ConfirmationCodeRepository(APIDbContext context) : base(context)
        {
        }
        public async Task<ConfirmationCode?> GetLatestConfirmationCodeByUserGuid(Guid userId)
        {
            return await _dbSet.Where(t => t.UserId == userId).OrderByDescending(t => t.DateTimeCreated).FirstOrDefaultAsync();
        }

        Task<ConfirmationCode?> IConfirmationCodeRepository.GetLatestConfirmationCodeByUserGuid(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
