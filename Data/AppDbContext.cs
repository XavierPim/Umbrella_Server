using Microsoft.EntityFrameworkCore;
using Umbrella_Server.Models;

namespace Umbrella_Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<Member> Members { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; } = null!;
        public DbSet<Attendee> Attendees { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraints
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Group>().HasIndex(g => g.GroupLink).IsUnique();

            // Composite unique key to prevent multiple membership records for same UserID in Group
            modelBuilder.Entity<Member>().HasIndex(m => new { m.GroupID, m.UserID }).IsUnique();

            // Bitmask for Admin permissions
            modelBuilder.Entity<Admin>().Property(a => a.Permissions).HasDefaultValue(0);

            // Spatial index for user location using SQL Server GEOGRAPHY
            modelBuilder.Entity<User>().HasIndex(u => u.Location);

            // Relationships
            modelBuilder.Entity<User>()
                .HasMany(u => u.Members)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Admins)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.AdminID);

            modelBuilder.Entity<User>()
                .HasOne(u => u.AttendeeInfo)
                .WithOne(a => a.User)
                .HasForeignKey<Attendee>(a => a.UserID);

            modelBuilder.Entity<Group>()
                .HasOne(g => g.Organizer)
                .WithMany()
                .HasForeignKey(g => g.OrganizerID);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Members)
                .WithOne(m => m.Group)
                .HasForeignKey(m => m.GroupID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
