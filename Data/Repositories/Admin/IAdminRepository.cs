using Umbrella_Server.Models;

namespace Umbrella_Server.Data.Repositories.Admin
{
    public interface IAdminRepository : IRepository<AdminUser>
    {
        Task<AdminUser?> GetAdminByUserIdAsync(Guid userId);
        Task<bool> UserIsAdminAsync(Guid userId);
    }
}
