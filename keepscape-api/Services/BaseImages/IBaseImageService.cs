using keepscape_api.Models;

namespace keepscape_api.Services.BaseImages
{
    public interface IBaseImageService
    {
        Task<BaseImage?> Get(string objectName, Guid id);
        Task<BaseImage?> Upload(string objectName, IFormFile file);
    }
}
