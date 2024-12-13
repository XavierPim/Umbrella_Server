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
        public DbSet<MemberLocation> MemberLocations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ SQL Server default for DateCreated
            modelBuilder.Entity<User>()
                .Property(u => u.DateCreated)
                .HasDefaultValueSql("GETUTCDATE()"); // Use GETUTCDATE() as the default value

            // Unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Group>()
                .HasIndex(g => g.GroupLink)
                .IsUnique();

            // Composite primary key for Member (GroupID + UserID)
            modelBuilder.Entity<Member>()
                .HasKey(m => new { m.GroupID, m.UserID });

            // Composite primary key for MemberLocation (GroupID + UserID + TimeStamp)
            modelBuilder.Entity<MemberLocation>()
                .HasKey(ml => new { ml.GroupID, ml.UserID, ml.TimeStamp });

            // Set Latitude and Longitude as required properties for MemberLocation
            modelBuilder.Entity<MemberLocation>()
                .Property(ml => ml.Latitude)
                .IsRequired();

            modelBuilder.Entity<MemberLocation>()
                .Property(ml => ml.Longitude)
                .IsRequired();

            // Set Latitude and Longitude as optional properties for User (user may not share location)
            modelBuilder.Entity<User>()
                .Property(u => u.Latitude)
                .HasColumnType("float");

            modelBuilder.Entity<User>()
                .Property(u => u.Longitude)
                .HasColumnType("float");

            // Set default value for Roles in User (default role is Attendee)
            modelBuilder.Entity<User>()
                .Property(u => u.Roles)
                .HasDefaultValue((int)UserRole.Attendee); // Default to Attendee role

            // Relationships
            modelBuilder.Entity<User>()
                .HasMany(u => u.Members)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.AttendeeInfo)
                .WithOne(a => a.User)
                .HasForeignKey<Attendee>(a => a.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Group>()
                .HasOne(g => g.Organizer)
                .WithMany()
                .HasForeignKey(g => g.OrganizerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Members)
                .WithOne(m => m.Group)
                .HasForeignKey(m => m.GroupID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MemberLocation>()
                .HasOne(ml => ml.User)
                .WithMany()
                .HasForeignKey(ml => ml.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MemberLocation>()
                .HasOne(ml => ml.Group)
                .WithMany()
                .HasForeignKey(ml => ml.GroupID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
