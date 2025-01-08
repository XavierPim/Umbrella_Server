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

        public async Task<Group?> GetGroupByLinkAsync(string groupLink)
        {
            return await _context.Groups
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.GroupLink == groupLink);
        }
    }
}
