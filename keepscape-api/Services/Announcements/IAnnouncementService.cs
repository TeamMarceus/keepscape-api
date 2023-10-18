using keepscape_api.Dtos.Announcements;
using keepscape_api.QueryModels;

namespace keepscape_api.Services.Announcements
{
    public interface IAnnouncementService
    {
        Task<AnnouncementResponsePagedDto> Get(PaginatorQuery paginatorQuery);
        Task<AnnouncementResponseDto?> GetById(Guid id);
        Task<AnnouncementResponseDto> Create(AnnouncementRequestDto announcementRequestDto);
        Task<bool> Delete(Guid id);
    }
}
