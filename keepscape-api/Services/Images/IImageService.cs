using keepscape_api.Models;

namespace keepscape_api.Services.BaseImages
{
    public interface IImageService
    {
        Task<string?> Get(string objectName, Guid id);
        Task<string?> Upload(string objectName, IFormFile file);
        Task<bool> Delete(string url);
    }
}
