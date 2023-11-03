using Azure.Storage.Blobs;
using Azure;
using Azure.Storage.Blobs.Models;

namespace keepscape_api.Services.BaseImages
{
    public class ImageService : IImageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "keepscapestorage";

        public ImageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string?> Get(string objectName, Guid id)
        {
            string objectPath = $"{objectName}/{id}";
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            BlobClient blobClient = containerClient.GetBlobClient(objectPath);

            try
            {
                var blobDownloadInfo = await blobClient.DownloadAsync();

                if (blobDownloadInfo != null)
                {
                    return blobClient.Uri.AbsoluteUri;
                }
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }

            return null;
        }

        public async Task<string?> Upload(string objectName, IFormFile file)
        {
            if (file.Length <= 0)
            {
                return null;
            }

            string objectPath = $"{objectName}/{Guid.NewGuid()}";
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            BlobClient blobClient = containerClient.GetBlobClient(objectPath);

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    BlobHttpHeaders headers = new BlobHttpHeaders
                    {
                        ContentType = file.ContentType
                    };

                    await blobClient.UploadAsync(stream, headers);
                    return blobClient.Uri.AbsoluteUri;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }

        public async Task<bool> Delete(string url)
        {
            Uri uri = new Uri(url);
            BlobClient blobClient = new BlobClient(uri);

            try
            {
                await blobClient.DeleteIfExistsAsync();
                return true;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return false;
            }
        }
    }
}
