using Microsoft.EntityFrameworkCore;
using Umbrella_Server.Data.Repositories.Admin;
using Umbrella_Server.Models;

namespace Umbrella_Server.Data.Repositories
{
    public class AdminRepository : Repository<AdminUser>, IAdminRepository
    {
        public AdminRepository(AppDbContext context) : base(context) { }


        // ✅ Get Admin by UserID
        public async Task<AdminUser?> GetAdminByUserIdAsync(Guid userId)
        {
            return await _context.Admins
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.UserID == userId);
        }

        // ✅ Check if User is Admin
        public async Task<bool> UserIsAdminAsync(Guid userId)
        {
            return await _context.Admins.AnyAsync(a => a.UserID == userId);
        }
    }
}
