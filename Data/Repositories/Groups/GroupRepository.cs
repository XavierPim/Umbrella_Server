using Microsoft.EntityFrameworkCore;
using Umbrella_Server.Models;

namespace Umbrella_Server.Data.Repositories.Groups
{
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        private readonly AppDbContext _context;

        public GroupRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // ✅ Get group by ID
        public async Task<Group?> GetGroupByIdAsync(Guid groupId)
        {
            return await _context.Groups
                .Include(g => g.Members)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(g => g.GroupID == groupId);
        }

        // ✅ Get group by invite link
        public async Task<Group?> GetGroupByLinkAsync(string groupLink)
        {
            return await _context.Groups
                .Include(g => g.Members)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(g => g.GroupLink == groupLink);
        }

        // ✅ Add a user to a group
        public async Task AddMemberAsync(Guid groupId, Guid userId, List<UserRole> roles)
        {
            var existingMember = await _context.Members
                .FirstOrDefaultAsync(m => m.GroupID == groupId && m.UserID == userId);

            if (existingMember == null)
            {
                var newMember = new Member
                {
                    GroupID = groupId,
                    UserID = userId,
                    Roles = roles
                };

                _context.Members.Add(newMember);
                await _context.SaveChangesAsync(); // ✅ Ensure it persists
            }
        }
    }
}
