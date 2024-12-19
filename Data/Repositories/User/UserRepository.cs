using Microsoft.EntityFrameworkCore;
using Umbrella_Server.Models;

namespace Umbrella_Server.Data.Repositories.User
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a user by email address (includes Members, AdminInfo, AttendeeInfo).
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Members)
                .Include(u => u.AdminInfo)
                .Include(u => u.AttendeeInfo)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
