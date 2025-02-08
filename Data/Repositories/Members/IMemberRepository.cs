using Umbrella_Server.Models;

namespace Umbrella_Server.Data.Repositories.Members
{
    public interface IMemberRepository
    {
        Task<IEnumerable<Member>> GetMembersByGroupIdAsync(Guid groupId);
        Task<Member?> GetMemberAsync(Guid groupId, Guid userId);
        Task AddMemberAsync(Member member);
        Task UpdateMemberAsync(Member member);
        Task RemoveMemberAsync(Member member);
        Task<bool> SaveChangesAsync();
    }
}
