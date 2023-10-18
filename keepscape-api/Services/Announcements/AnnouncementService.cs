using keepscape_api.Dtos.Announcements;
using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Interfaces;

namespace keepscape_api.Services.Announcements
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IAnnouncementRepository _announcementRepository;

        public AnnouncementService(IAnnouncementRepository announcementRepository)
        {
            _announcementRepository = announcementRepository;
        }

        public async Task<AnnouncementResponseDto> Create(AnnouncementRequestDto announcementRequestDto)
        {
            var announcement = new Announcement
            {
                Title = announcementRequestDto.Title,
                Description = announcementRequestDto.Description,
            };

            var createdAnnouncement = await _announcementRepository.AddAsync(announcement);

            return new AnnouncementResponseDto
            {
                Id = createdAnnouncement.Id,
                Title = createdAnnouncement.Title ?? "",
                Description = createdAnnouncement.Description ?? "",
                DateTimeCreated = createdAnnouncement.DateTimeCreated
            };

        }

        public async Task<bool> Delete(Guid id)
        {
            return await _announcementRepository.DeleteAsync(new Announcement { Id = id });
        }

        public async Task<AnnouncementResponseDto?> GetById(Guid id)
        {
            var announcement = await _announcementRepository.GetByIdAsync(id);

            if (announcement == null)
            {
                return null;
            }

            return new AnnouncementResponseDto
            {
                Id = announcement.Id,
                Title = announcement.Title ?? "",
                Description = announcement.Description ?? "",
                DateTimeCreated = announcement.DateTimeCreated
            };
        }

        public async Task<AnnouncementResponsePagedDto> Get(PaginatorQuery paginatorQuery)
        {
            var queryResult = await _announcementRepository.GetAnnouncementsAsync(paginatorQuery);

            return new AnnouncementResponsePagedDto
            {
                Announcements = queryResult.announcements.Select(a => new AnnouncementResponseDto
                {
                    Id = a.Id,
                    Title = a.Title ?? "",
                    Description = a.Description ?? "",
                    DateTimeCreated = a.DateTimeCreated
                }),
                PageCount = queryResult.pageCount
            };
        }
    }
}
