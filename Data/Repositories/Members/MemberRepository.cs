using Microsoft.EntityFrameworkCore;
using Umbrella_Server.Models;

namespace Umbrella_Server.Data.Repositories.Members
{
    public class MemberRepository : IMemberRepository
    {
        private readonly AppDbContext _context;

        public MemberRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Member>> GetMembersByGroupIdAsync(Guid groupId)
        {
            return await _context.Members.Where(m => m.GroupID == groupId).ToListAsync();
        }

        public async Task<Member?> GetMemberAsync(Guid groupId, Guid userId)
        {
            return await _context.Members.FirstOrDefaultAsync(m => m.GroupID == groupId && m.UserID == userId);
        }

        public async Task AddMemberAsync(Member member)
        {
            await _context.Members.AddAsync(member);
        }

        public async Task UpdateMemberAsync(Member member)
        {
            _context.Members.Update(member);
            await _context.SaveChangesAsync(); 
        }

        public async Task RemoveMemberAsync(Member member)
        {
            _context.Members.Remove(member);
            await _context.SaveChangesAsync(); 
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
