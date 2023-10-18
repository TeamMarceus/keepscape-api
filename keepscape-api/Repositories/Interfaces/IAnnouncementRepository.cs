using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface IAnnouncementRepository : IBaseRepository<Announcement>
    {
        Task<int> GetAnnouncementCountAsync();
        Task<(IEnumerable<Announcement> announcements, int pageCount)> GetAnnouncementsAsync(PaginatorQuery paginatorQuery);
    }
}
