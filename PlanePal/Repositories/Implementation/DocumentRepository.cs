using PlanePal.DbContext;
using PlanePal.Model.UserModel;
using PlanePal.Repositories.Interfaces;

namespace PlanePal.Repositories.Implementation
{
    public class DocumentRepository : BaseRepository<IdentificationDocument, int>, IDocumentRepository
    {
        public DocumentRepository(PlanePalDbContext dataContext) : base(dataContext)
        {
        }
    }
}