using PlanePal.DbContext;
using PlanePal.Model.UserModel;
using PlanePal.Repositories.Interfaces;

namespace PlanePal.Repositories.Implementation
{
    public class UserRoleRepository : BaseRepository<UserRole, int>, IUserRoleRepository
    {
        public UserRoleRepository(PlanePalDbContext dataContext) : base(dataContext)
        {
        }
    }
}