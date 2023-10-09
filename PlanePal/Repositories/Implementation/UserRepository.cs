using Microsoft.EntityFrameworkCore;
using PlanePal.DbContext;
using PlanePal.DTOs.User;
using PlanePal.Model.UserModel;
using PlanePal.Repositories.Interfaces;

namespace PlanePal.Repositories.Implementation
{
    public class UserRepository : BaseRepository<User, string>, IUserRepository
    {
        private readonly PlanePalDbContext _context;

        public UserRepository(PlanePalDbContext dataContext) : base(dataContext)
        {
            _context = dataContext;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _dataContext.Set<User>()
                .Include("Address")
                .Include("IdentificationDocument")
                .Include("UserRole")
                .DefaultIfEmpty()
                .ToListAsync();
        }

        public async Task<User> GetOneWithAddressIdAndDocumentId(string email)
        {
            return await _context.Users
                .Include(u => u.Address)
                .Include(u => u.IdentificationDocument)

                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByEmail(LoginUserDTO login)
        {
            return await _context.Users.FirstOrDefaultAsync((User u) => u.Email == login.Email);
        }
    }
}