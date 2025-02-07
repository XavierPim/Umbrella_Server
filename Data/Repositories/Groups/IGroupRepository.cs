using Umbrella_Server.Models;

namespace Umbrella_Server.Data.Repositories.Groups
{
    public interface IGroupRepository : IRepository<Group>
    {
        Task<Group?> GetGroupByIdAsync(Guid groupId); 
        Task<Group?> GetGroupByLinkAsync(string groupLink);
        Task AddMemberAsync(Guid groupId, Guid userId, List<UserRole> roles);
    }
}
