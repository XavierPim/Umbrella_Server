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
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public override async Task<User?> GetByIdAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.AdminInfo)
                .FirstOrDefaultAsync(u => u.UserID == userId);
        }
    }
}
