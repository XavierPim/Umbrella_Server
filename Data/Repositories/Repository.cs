
using Microsoft.EntityFrameworkCore;

namespace Umbrella_Server.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;

    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context; // Allow child classes to access the context
        protected readonly DbSet<T> _dbSet; // DB Set for T (User, Group, etc.)

        // ✅ Constructor to accept the context and initialize the DB set
        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>(); // Automatically set the DbSet for the specific model
        }

        // ✅ Get a single item by its ID
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id); // Simple EF method to find by primary key
        }

        // ✅ Get all items in the table
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync(); // Get all records in the table
        }

        // ✅ Add a new entity
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity); // Add to context
            await _context.SaveChangesAsync(); // Save to database
        }

        // ✅ Update an existing entity
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity); // Mark as updated
            await _context.SaveChangesAsync(); // Save changes to the database
        }

        // ✅ Delete an entity by ID
        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id); // Get the entity by its ID
            if (entity != null)
            {
                _dbSet.Remove(entity); // Remove the entity
                await _context.SaveChangesAsync(); // Save changes
            }
        }
    }

}

