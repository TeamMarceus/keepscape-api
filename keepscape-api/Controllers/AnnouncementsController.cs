using keepscape_api.Dtos.Announcements;
using keepscape_api.QueryModels;
using keepscape_api.Services.Announcements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace keepscape_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<IActionResult> GetAnnouncement(Guid id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var announcement = await _announcementService.GetById(id);

                return Ok(announcement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_announcementService.Get)} threw an exception");
                return StatusCode(500, "Error getting announcement");
            }
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
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
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteAnnouncement(Guid id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var isDeleted = await _announcementService.Delete(id);

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
