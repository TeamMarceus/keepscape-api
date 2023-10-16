using keepscape_api.Data;
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

        public async Task<IEnumerable<SellerApplication>> Get(PaginatorQuery paginatorQuery)
        {
            if (paginatorQuery.Page == null || paginatorQuery.PageSize == null)
            {
                return await _dbSet
                    .Include(b => b.BaseImage)
                    .ToListAsync();
            }

            return await _dbSet
                .Include(b => b.BaseImage)
                .Skip(paginatorQuery.Page.Value * paginatorQuery.PageSize.Value)
                .Take(paginatorQuery.PageSize.Value)
                .ToListAsync();
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
                .Include(b => b.BaseImage)
                .Include(b => b.SellerProfile)
                .FirstOrDefaultAsync(b => b.SellerProfile!.UserId == userId);
        }
    }
}
