using Microsoft.EntityFrameworkCore;
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
            // 🔥 USER CONFIGURATION
            // ======================================
            modelBuilder.Entity<User>()
                .Property(u => u.DateCreated)
                .HasDefaultValueSql("GETUTCDATE()"); // SQL Server will handle the default value for DateCreated

            modelBuilder.Entity<User>()
                .HasMany(u => u.Members)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserID)
                .OnDelete(DeleteBehavior.Restrict); // Restrict deletion of User if they are referenced in Members

            modelBuilder.Entity<User>()
                .HasOne(u => u.AttendeeInfo)
                .WithOne(a => a.User)
                .HasForeignKey<Attendee>(a => a.UserID)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete the Attendee if User is deleted


            modelBuilder.Entity<User>()
                .HasOne(u => u.AdminInfo)
                .WithOne(a => a.User)
                .HasForeignKey<Admin>(a => a.UserID)
                .OnDelete(DeleteBehavior.Cascade); // If User is deleted, delete related Admin

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); // Unique email constraint

            modelBuilder.Entity<User>()
       .Property(u => u.Roles)
       .HasConversion(
           v => string.Join(',', v),      // Convert List<UserRole> to a CSV string
           v => v.Split(',', StringSplitOptions.RemoveEmptyEntries) // Convert CSV back to List<UserRole>
                 .Select(r => Enum.Parse<UserRole>(r))
                 .ToList());



            // ======================================
            // 🔥 ADMIN CONFIGURATION
            // ======================================
            modelBuilder.Entity<Admin>()
                .Property(a => a.Permissions)
                .HasDefaultValue(AdminPermissions.None); // Default permissions set to "None"

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne(u => u.AdminInfo)
                .HasForeignKey<Admin>(a => a.UserID)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for related Admin

            // ======================================
            // 🔥 ATTENDEE CONFIGURATION
            // ======================================
            modelBuilder.Entity<Attendee>()
                .HasOne(a => a.User)
                .WithOne(u => u.AttendeeInfo)
                .HasForeignKey<Attendee>(a => a.UserID)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete the Attendee if User is deleted

            modelBuilder.Entity<Attendee>()
                .Property(a => a.CanMessage)
                .HasDefaultValue(false); // Default value for CanMessage is false

            modelBuilder.Entity<Attendee>()
                .Property(a => a.CanCall)
                .HasDefaultValue(false); // Default value for CanCall is false

            modelBuilder.Entity<Attendee>()
                .Property(a => a.RsvpStatus)
                .HasDefaultValue(RsvpStatus.Pending); // Default RSVP status is "Pending"

            // ======================================
            // 🔥 GROUP CONFIGURATION
            // ======================================
            modelBuilder.Entity<Group>()
                .Property(g => g.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()"); // CreatedAt defaults to current UTC time

            modelBuilder.Entity<Group>()
                .Property(g => g.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("GETUTCDATE()"); // UpdatedAt is updated whenever the row is modified

            modelBuilder.Entity<Group>()
                .HasOne(g => g.Organizer)
                .WithMany()
                .HasForeignKey(g => g.OrganizerID)
                .OnDelete(DeleteBehavior.Restrict); // If the organizer is deleted, groups are NOT deleted

            modelBuilder.Entity<Group>()
                .HasIndex(g => g.GroupLink)
                .IsUnique(); // Ensure unique GroupLink constraint

            // ======================================
            // 🔥 MEMBER CONFIGURATION
            // ======================================
            modelBuilder.Entity<Member>()
                .HasKey(m => new { m.GroupID, m.UserID }); // Composite PK for GroupID + UserID

            modelBuilder.Entity<Member>()
                .HasOne(m => m.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(m => m.GroupID)
                .OnDelete(DeleteBehavior.Cascade); // Delete Member if Group is deleted

            modelBuilder.Entity<Member>()
                .HasOne(m => m.User)
                .WithMany(u => u.Members)
                .HasForeignKey(m => m.UserID)
                .OnDelete(DeleteBehavior.Restrict); // Prevent User deletion if they are part of a Member
        }
    }
}
