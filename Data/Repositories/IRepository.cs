namespace Umbrella_Server.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id); // Get one by ID
        Task<IEnumerable<T>> GetAllAsync(); // Get all records
        Task AddAsync(T entity); // Add new entity
        Task UpdateAsync(T entity); // Update existing entity
        Task DeleteAsync(Guid id); // Delete entity by ID
    }

}
