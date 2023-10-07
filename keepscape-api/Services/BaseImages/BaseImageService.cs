using Google.Cloud.Storage.V1;
using keepscape_api.Models;

namespace keepscape_api.Services.BaseImages
{
    public class BaseImageService : IBaseImageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName = "keepscape-storage";
        public Task<BaseImage?> Get(string objectName, Guid guid)
        {
            throw new NotImplementedException();
        }

        public Task<BaseImage?> Upload(string objectName, Guid guid, IFormFile file)
        {
            throw new NotImplementedException();
        }
    }
}
