using Umbrella_Server.Models;

namespace Umbrella_Server.Data.Repositories.Groups
{
    public interface IGroupRepository : IRepository<Group>
    {
        Task<Group?> GetGroupByLinkAsync(string groupLink);
    }
}
