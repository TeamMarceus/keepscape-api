using Google.Cloud.Storage.V1;
using keepscape_api.Models;

namespace keepscape_api.Services.BaseImages
{
    public class BaseImageService : IBaseImageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName = "keepscape_storage";

        public BaseImageService(StorageClient storageClient)
        {
            _storageClient = storageClient;
        }
        public async Task<BaseImage?> Get(string objectName, Guid id)
        {
            string objectPath = $"{objectName}/{id}";

            try
            {
                var obj = await _storageClient.GetObjectAsync(_bucketName, objectPath);

                if (obj != null)
                {
                    return new BaseImage
                    {
                        Id = id,
                        Alt = obj.Name,
                        Url = obj.MediaLink
                    };
                }
            }
            catch (Google.GoogleApiException ex) when (ex.Error.Code == 404)
            {
                return null;
            }

            return null;
        }

        public async Task<BaseImage?> Upload(string objectName, IFormFile file)
        {
            string objectPath = $"{objectName}/{Guid.NewGuid()}";

            if (file.Length <= 0)
            {
                return null;
            }

            using (var stream = file.OpenReadStream())
            {
                var obj = await _storageClient.
                    UploadObjectAsync(_bucketName, objectPath, file.ContentType, stream);

                return new BaseImage
                {
                    Alt = obj.Name,
                    Url = $"https://storage.googleapis.com/{_bucketName}/{objectPath}"
                };
            }
        }

        public async Task<bool> Delete(string url)
        {
            string objectPath = url.Split($"{_bucketName}/")[1];

            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, objectPath);
                return true;
            }
            catch (Google.GoogleApiException ex) when (ex.Error.Code == 404)
            {
                return false;
            }
        }
    }
}
