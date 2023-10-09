using Azure;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using PlanePal.DTOs.Blob;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Shared;

namespace PlanePal.Repositories.Implementation
{
    public class AzureStorage : IAzureStorage
    {
        private readonly KeyVaultSecret _storageConnectionString;
        private readonly KeyVaultSecret _storageContainerName;
        private readonly ILogger<AzureStorage> _logger;
        private readonly SecretClient _secretClient;

        public AzureStorage(ILogger<AzureStorage> logger)
        {
            _logger = logger;
            _secretClient = AzureKeyVaultClientProvider.GetClient();
            _storageConnectionString = _secretClient.GetSecret("storage-Connection-string");
            _storageContainerName = _secretClient.GetSecret("Storage-account-name");
        }

        public async Task<BlobResponseDTO> UploadAsync(IFormFile file)
        {
            BlobResponseDTO response = new();
            var uniqueFileName = Guid.NewGuid().ToString();

            BlobContainerClient container = new(_storageConnectionString.Value, _storageContainerName.Value);
            try
            {
                BlobClient client = container.GetBlobClient(uniqueFileName);

                await using (Stream data = file.OpenReadStream())
                {
                    await client.UploadAsync(data);
                }

                response.Status = $"File {uniqueFileName} uploaded successfully";
                response.Error = false;
                response.Blob.Uri = client.Uri.AbsoluteUri;
                response.Blob.Name = client.Name;
            }
            catch (RequestFailedException ex)
               when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
            {
                _logger.LogError($"File with name {uniqueFileName} already exists in container.");
                response.Status = $"File with name {uniqueFileName} already exists. Please use another name to store your file.";
                response.Error = true;
                return response;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
                response.Status = $"Unexpected error: {ex.StackTrace}. Check log with StackTrace ID.";
                response.Error = true;
                return response;
            }
            return response;
        }

        public async Task<BlobResponseDTO> DeleteAsync(string blobFilename)
        {
            BlobContainerClient client = new(_storageConnectionString.Value, _storageContainerName.Value);

            BlobClient file = client.GetBlobClient(blobFilename);

            try
            {
                await file.DeleteAsync();
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                _logger.LogError($"File {blobFilename} does not exists.");
                return new BlobResponseDTO { Error = true, Status = $"File {blobFilename} does not exists." };
            }

            return new BlobResponseDTO { Error = false, Status = $"File: {blobFilename} has been successfully deleted." };
        }

        public async Task<BlobDTO> DownloadAsync(string blobFilename)
        {
            BlobContainerClient client = new(_storageConnectionString.Value, _storageContainerName.Value);

            try
            {
                BlobClient file = client.GetBlobClient(blobFilename);

                if (await file.ExistsAsync())
                {
                    var content = await file.DownloadContentAsync();

                    string name = blobFilename;
                    string contentType = content.Value.Details.ContentType;

                    return new BlobDTO { Name = name, ContentType = contentType };
                }
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                _logger.LogError($"File {blobFilename} was not found.");
            }
            return null;
        }
    }
}