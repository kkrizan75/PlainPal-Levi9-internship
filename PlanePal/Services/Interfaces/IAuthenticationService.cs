using PlanePal.DTOs.User;
using PlanePal.Model.UserModel;

namespace PlanePal.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<User> Authenticate(LoginUserDTO login);

        string GenerateToken(User user);
    }
}