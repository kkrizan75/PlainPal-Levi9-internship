using PlanePal.DTOs.Blob;

namespace PlanePal.Repositories.Interfaces
{
    public interface IAzureStorage
    {
        Task<BlobResponseDTO> UploadAsync(IFormFile file);

        Task<BlobDTO> DownloadAsync(string blobFilename);

        Task<BlobResponseDTO> DeleteAsync(string blobFilename);
    }
}