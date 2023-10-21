using Google.Cloud.Storage.V1;

namespace keepscape_api.Services.BaseImages
{
    public class ImageService : IImageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName = "keepscape_storage";

        public ImageService(StorageClient storageClient)
        {
            _storageClient = storageClient;
        }
        public async Task<string?> Get(string objectName, Guid id)
        {
            string objectPath = $"{objectName}/{id}";

            try
            {
                var obj = await _storageClient.GetObjectAsync(_bucketName, objectPath);

                if (obj != null)
                {
                    return obj.MediaLink;
                }
            }
            catch (Google.GoogleApiException ex) when (ex.Error.Code == 404)
            {
                return null;
            }

            return null;
        }

        public async Task<string?> Upload(string objectName, IFormFile file)
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

                return $"https://storage.googleapis.com/{_bucketName}/{objectPath}";
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
