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

        public async Task<(IEnumerable<SellerApplication> SellerApplications, int PageCount)> Get(SellerApplicationQuery sellerApplicationQuery)
        {
            var query = _dbSet
                .Include(s => s.SellerProfile)
                    .ThenInclude(s => s.User)
                .AsQueryable();

            int pageCount = 1;

            if (!string.IsNullOrEmpty(sellerApplicationQuery.OrderBy))
            {
                if (sellerApplicationQuery.OrderBy == "DateTimeCreated")
                {
                    if (sellerApplicationQuery.IsDescending)
                    {
                        query = query.OrderByDescending(s => s.DateTimeCreated);
                    }
                    else
                    {
                        query = query.OrderBy(s => s.DateTimeCreated);
                    }
                }
                else if (sellerApplicationQuery.OrderBy == "DateTimeApproved")
                {
                    if (sellerApplicationQuery.IsDescending)
                    {
                        query = query.OrderByDescending(s => s.DateTimeUpdated);
                    }
                    else
                    {
                        query = query.OrderBy(s => s.DateTimeUpdated);
                    }
                }
            }

            if (!string.IsNullOrEmpty(sellerApplicationQuery.Status))
            {
                if (Enum.TryParse<ApplicationStatus>(sellerApplicationQuery.Status, out var status))
                {
                    query = query.Where(b => b.Status == status);
                }
            }


            if (query.Count() == 0)
            {
                return (new List<SellerApplication>(), 0);
            }

            if (sellerApplicationQuery.Page != null && sellerApplicationQuery.PageSize != null)
            {
                int queryPageCount = await query.CountAsync();

                pageCount = (int)Math.Ceiling((double)queryPageCount / (int)sellerApplicationQuery.PageSize);

                if (sellerApplicationQuery.Page > pageCount)
                {
                    sellerApplicationQuery.Page = pageCount;
                }
                else if (sellerApplicationQuery.Page < 1)
                {
                    sellerApplicationQuery.Page = 1;
                }
                else if (pageCount == 0)
                {
                    pageCount = 1;
                    sellerApplicationQuery.Page = 1;
                }

                query = query
                    .Skip((int) ((sellerApplicationQuery.Page - 1) * sellerApplicationQuery.PageSize))
                    .Take((int) sellerApplicationQuery.PageSize);
            }
           
            return (await query.ToListAsync(), pageCount);
        }

        public override async Task<IEnumerable<SellerApplication>> GetAllAsync()
        {
            return await _dbSet
                .ToListAsync();
        }
        public override async Task<SellerApplication?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(s => s.SellerProfile)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public Task<SellerApplication?> GetBySellerProfileId(Guid sellerProfileId)
        {
            return _dbSet
                .FirstOrDefaultAsync(b => b.SellerProfileId == sellerProfileId);
        }

        public Task<SellerApplication?> GetByUserId(Guid userId)
        {
            return _dbSet
                .Include(s => s.SellerProfile)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(b => b.SellerProfile!.UserId == userId);
        }
    }
}
