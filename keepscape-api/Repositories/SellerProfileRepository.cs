using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.Repositories.Generics;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class SellerProfileRepository : BaseRepository<SellerProfile>, ISellerProfileRepository
    {
        public SellerProfileRepository(APIDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<SellerProfile>> GetAllAsync()
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.SellerApplication)
                    .ThenInclude(s => s!.BaseImage)
                .ToListAsync();
        }

        public async Task<IEnumerable<SellerProfile>> GetAllPagedAsync(int pageIndex, int pageSize)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.SellerApplication)
                    .ThenInclude(s => s!.BaseImage)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public override async Task<SellerProfile?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.SellerApplication)
                    .ThenInclude(s => s!.BaseImage)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<SellerProfile?> GetProfileByUserGuid(Guid userId)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.SellerApplication)
                    .ThenInclude(s => s!.BaseImage)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public Task<int> GetTotalProfileCount()
        {
            return _dbSet.CountAsync();
        }
    }
}
