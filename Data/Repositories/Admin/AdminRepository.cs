using Microsoft.EntityFrameworkCore;
using Umbrella_Server.Models;

namespace Umbrella_Server.Data.Repositories.Admin
{
    public class AdminRepository : Repository<AdminUser>, IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // ✅ Get Admin by UserID (Admin can only exist if they're a user)
        public async Task<AdminUser?> GetAdminByUserIdAsync(Guid userId)
        {
            return await _context.Admins
                .Include(a => a.User) // Optional if you want User details included
                .FirstOrDefaultAsync(a => a.UserID == userId);
        }

        // ✅ Check if User is Admin
        public async Task<bool> UserIsAdminAsync(Guid userId)
        {
            return await _context.Admins.AnyAsync(a => a.UserID == userId);
        }
    }
}
