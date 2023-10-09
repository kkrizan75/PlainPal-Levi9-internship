using Azure.Security.KeyVault.Secrets;
using Microsoft.IdentityModel.Tokens;
using PlanePal.DTOs.User;
using PlanePal.Model.UserModel;
using PlanePal.Repositories.Interfaces;
using PlanePal.Services.Interfaces;
using PlanePal.Services.Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PlanePal.Services.Implementation
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public AuthenticationService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        private bool ComparePasswords(string inputPassword, string storedPasswordHash, string storedSalt)
        {
            return storedPasswordHash.CompareTo(HashnSaltService.HashWithSalt(inputPassword, storedSalt)) == 0;
        }

        public async Task<User> Authenticate(LoginUserDTO login)
        {
            var currentUser = await _unitOfWork.UserRepository.GetByEmail(login);

            if (currentUser is null || !ComparePasswords(login.Password, currentUser.Password, currentUser.Salt))
            {
                return null;
            }
            return currentUser;
        }

        public string GenerateToken(User user)
        {
            KeyVaultSecret jwtKey = AzureKeyVaultClientProvider.GetClient().GetSecret("Jwt--Key");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey.Value));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var role = _unitOfWork.UserRoleRepository.GetOne(user.RoleId);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role.Name)
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}