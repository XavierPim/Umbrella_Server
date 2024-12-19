using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Umbrella_Server.Models;

namespace Umbrella_Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet properties for all models
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; } = null!;
        public DbSet<Attendee> Attendees { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<Member> Members { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ======================================
            // USER CONFIGURATION
            // ======================================
            var userRoleComparer = new ValueComparer<List<UserRole>>(
                (list1, list2) => list1.SequenceEqual(list2),
                list => list.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                list => list.ToList()
            );

            modelBuilder.Entity<User>()
                .Property(u => u.DateCreated)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Roles)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(r => Enum.Parse<UserRole>(r))
                          .ToList()
                )
                .Metadata.SetValueComparer(userRoleComparer);


            modelBuilder.Entity<User>()
                .HasMany(u => u.Members)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // ======================================
            // ADMIN CONFIGURATION
            // ======================================
            modelBuilder.Entity<Admin>()
                .Property(a => a.Permissions)
                .HasDefaultValue(AdminPermissions.None);

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne(u => u.AdminInfo)
                .HasForeignKey<Admin>(a => a.UserID)
                .OnDelete(DeleteBehavior.Restrict); // Restrict deletion

            // ======================================
            // ATTENDEE CONFIGURATION
            // ======================================
            modelBuilder.Entity<Attendee>()
                .Property(a => a.CanMessage)
                .HasDefaultValue(false);

            modelBuilder.Entity<Attendee>()
                .Property(a => a.CanCall)
                .HasDefaultValue(false);

            modelBuilder.Entity<Attendee>()
                .Property(a => a.RsvpStatus)
                .HasDefaultValue(RsvpStatus.Pending);

            modelBuilder.Entity<Attendee>()
                .HasOne(a => a.User)
                .WithOne(u => u.AttendeeInfo)
                .HasForeignKey<Attendee>(a => a.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // ======================================
            // GROUP CONFIGURATION
            // ======================================
            modelBuilder.Entity<Group>()
                .Property(g => g.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Group>()
                .Property(g => g.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Group>()
                .HasIndex(g => g.GroupLink)
                .IsUnique();

            modelBuilder.Entity<Group>()
                .HasOne(g => g.Organizer)
                .WithMany()
                .HasForeignKey(g => g.OrganizerID)
                .OnDelete(DeleteBehavior.Restrict);

            // ======================================
            // MEMBER CONFIGURATION
            // ======================================
            modelBuilder.Entity<Member>()
                .HasKey(m => new { m.GroupID, m.UserID });

            modelBuilder.Entity<Member>()
                .HasOne(m => m.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(m => m.GroupID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Member>()
                .HasOne(m => m.User)
                .WithMany(u => u.Members)
                .HasForeignKey(m => m.UserID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
