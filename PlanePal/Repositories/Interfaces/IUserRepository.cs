using PlanePal.DTOs.User;
using PlanePal.Model.UserModel;

namespace PlanePal.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<User, string>
    {
        Task<IEnumerable<User>> GetAllUsers();

        Task<User> GetOneWithAddressIdAndDocumentId(string email);

        Task<User> GetByEmail(LoginUserDTO login);
    }
}