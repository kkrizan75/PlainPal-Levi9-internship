using PlanePal.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanePal.Model.UserModel
{
    public class IdentificationDocument
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key()]
        public int Id { get; set; }

        public IdDocumentTypeEnum DocumentType { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string DocumentNumber { get; set; }
        public string DocumentImageUri { get; set; }
    }
}