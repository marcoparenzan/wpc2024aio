using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Wpc2024HMIApp.Options;

namespace Wpc2024HMIApp.Services
{
    public class StorageService : IStorageService
    {
        private readonly StorageOptions _storageOptions;
        private BlobServiceClient blobServiceClient;

        public StorageService(
            IOptions<StorageOptions> storageOptions
            )
        {
            _storageOptions = storageOptions.Value;
            blobServiceClient = new BlobServiceClient(_storageOptions.ConnectionString);
        }

        public async Task UploadBlobAsync(string sourceFilePath, string containerName, string blobName)
        {
            var stream = File.OpenRead(sourceFilePath);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(stream, overwrite: true);
        }
    }
}
