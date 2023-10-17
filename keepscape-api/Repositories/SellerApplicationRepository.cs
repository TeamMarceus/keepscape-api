using keepscape_api.Data;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class SellerApplicationRepository : BaseRepository<SellerApplication>, ISellerApplicationRepository
    {
        public SellerApplicationRepository(APIDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SellerApplication>> Get(SellerApplicationQuery sellerApplicationQuery)
        {
            var query = _dbSet
                .Include(b => b.BaseImage)
                .Include(s => s.SellerProfile)
                    .ThenInclude(s => s.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(sellerApplicationQuery.Status))
            {
                if (Enum.TryParse<ApplicationStatus>(sellerApplicationQuery.Status, out var status))
                {
                    query = query.Where(b => b.Status == status);
                }
            }

            if (sellerApplicationQuery.Page != null && sellerApplicationQuery.PageSize != null)
            {
                query = query
                    .Skip((int) ((sellerApplicationQuery.Page - 1) * sellerApplicationQuery.PageSize))
                    .Take((int) sellerApplicationQuery.PageSize);
            }
           
            return await query.ToListAsync();
        }

        public override async Task<IEnumerable<SellerApplication>> GetAllAsync()
        {
            return await _dbSet
                .Include(b => b.BaseImage)
                .ToListAsync();
        }
        public override async Task<SellerApplication?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(b => b.BaseImage)
                .Include(s => s.SellerProfile)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public Task<SellerApplication?> GetBySellerProfileId(Guid sellerProfileId)
        {
            return _dbSet
                .Include(b => b.BaseImage)
                .FirstOrDefaultAsync(b => b.SellerProfileId == sellerProfileId);
        }

        public Task<SellerApplication?> GetByUserId(Guid userId)
        {
            return _dbSet
                .Include(s => s.BaseImage)
                .Include(s => s.SellerProfile)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(b => b.SellerProfile!.UserId == userId);
        }
    }
}
