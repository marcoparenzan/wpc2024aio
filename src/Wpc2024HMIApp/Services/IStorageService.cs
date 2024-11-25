
namespace Wpc2024HMIApp.Services
{
    public interface IStorageService
    {
        Task UploadBlobAsync(string sourceFilePath, string containerName, string blobName);
    }
}