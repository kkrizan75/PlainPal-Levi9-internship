using PlanePal.DTOs.Blob;
using PlanePal.Enums;

namespace PlanePal.DTOs.User
{
    public class DocumentDetailsDTO
    {
        public IdDocumentTypeEnum DocumentType { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string DocumentNumber { get; set; }
        public BlobDTO BlobDto { get; set; }
    }
}