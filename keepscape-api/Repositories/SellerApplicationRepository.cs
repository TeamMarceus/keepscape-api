using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class SellerApplicationRepository : BaseRepository<SellerApplication>, ISellerApplicationRepository
    {
        public SellerApplicationRepository(APIDbContext context) : base(context)
        {
        }
        public override async Task<IEnumerable<SellerApplication>> GetAllAsync()
        {
            return await _dbSet
                .Include(b => b.BaseImage)
                .ToListAsync();
        }

        public async Task<IEnumerable<SellerApplication>> GetAllPagedAsync(int pageIndex, int pageSize)
        {
            return await _dbSet
                .Include(b => b.BaseImage)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public override async Task<SellerApplication?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(b => b.BaseImage)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public Task<SellerApplication?> GetSellerApplicationBySellerProfileId(Guid sellerProfileId)
        {
            return _dbSet
                .Include(b => b.BaseImage)
                .FirstOrDefaultAsync(b => b.SellerProfileId == sellerProfileId);
        }

        public Task<SellerApplication?> GetSellerApplicationByUserId(Guid userId)
        {
            return _dbSet
                .Include(b => b.BaseImage)
                .Include(b => b.SellerProfile)
                .FirstOrDefaultAsync(b => b.SellerProfile!.UserId == userId);
        }
    }
}
