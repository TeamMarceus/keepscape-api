using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class AnnouncementRepository : BaseRepository<Announcement>, IAnnouncementRepository
    {
        public AnnouncementRepository(APIDbContext context) : base(context)
        {
        }

        public Task<int> GetAnnouncementCountAsync()
        {
            return _dbSet.CountAsync();
        }

        public override async Task<IEnumerable<Announcement>> GetAllAsync()
        {
            return await _dbSet
                .ToListAsync();
        }

        public override async Task<Announcement?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<(IEnumerable<Announcement> announcements, int pageCount)> GetAnnouncementsAsync(PaginatorQuery paginatorQuery)
        {
            var query = _dbSet.AsQueryable();

            if (query.Count() == 0)
            {
                return (Enumerable.Empty<Announcement>(), 0);
            }

            int pageCount = 1;
            if (paginatorQuery.Page > 0 && paginatorQuery.PageSize > 0)
            {
                pageCount = (int)Math.Ceiling((double)query.Count() / paginatorQuery.PageSize.Value);

                query = query
                    .Skip(paginatorQuery.Page.Value * paginatorQuery.PageSize.Value)
                    .Take(paginatorQuery.PageSize.Value);
            }

            return (await query.ToListAsync(), pageCount);
        }
    }
}
