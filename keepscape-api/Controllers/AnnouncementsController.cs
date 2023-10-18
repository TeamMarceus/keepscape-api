using keepscape_api.Dtos.Announcements;
using keepscape_api.QueryModels;
using keepscape_api.Services.Announcements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace keepscape_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Admin")]
    public class AnnouncementsController : ControllerBase
    {
        private readonly ILogger<AnnouncementService> _logger;
        private readonly IAnnouncementService _announcementService;

        public AnnouncementsController(ILogger<AnnouncementService> logger, IAnnouncementService announcementService)
        {
            _logger = logger;
            _announcementService = announcementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAnnouncements([FromQuery]PaginatorQuery paginatorQuery)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var announcements = await _announcementService.Get(paginatorQuery);

                return Ok(announcements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_announcementService.Get)} threw an exception");
                return StatusCode(500, "Error getting announcements");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnnouncement(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var guid = Guid.Parse(id.ToString());

                var announcement = await _announcementService.GetById(guid);

                return Ok(announcement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_announcementService.Get)} threw an exception");
                return StatusCode(500, "Error getting announcement");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnnouncement([FromBody]AnnouncementRequestDto announcementRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var announcement = await _announcementService.Create(announcementRequestDto);

                return Ok(announcement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_announcementService.Create)} threw an exception");
                return StatusCode(500, "Error creating announcement");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var guid = Guid.Parse(id.ToString());

                var isDeleted = await _announcementService.Delete(guid);

                return Ok(isDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_announcementService.Delete)} threw an exception");
                return StatusCode(500, "Error deleting announcement");
            }
        }
    }
}
