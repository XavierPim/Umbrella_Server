using Umbrella_Server.Models;

namespace Umbrella_Server.Data.Repositories.Users
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
    }
}
