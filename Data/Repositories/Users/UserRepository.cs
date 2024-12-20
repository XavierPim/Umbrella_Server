using Microsoft.EntityFrameworkCore;
using Umbrella_Server.Models;

namespace Umbrella_Server.Data.Repositories.Users
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking() 
                .Include(u => u.AdminInfo)
                .Include(u => u.AttendeeInfo)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
