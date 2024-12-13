using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Umbrella_Server.Models
{
    public class User
    {
        [Key]
        public Guid UserID { get; set; } = Guid.NewGuid();

        [Required]
        public required string Name { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }

        [Required]
        public int Roles { get; set; } = (int)UserRole.Attendee; // Default role is Attendee

        // Location tracking using simple latitude/longitude
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string GroupLink { get; set; } = string.Empty; // Invite code (e.g., ABC123)

        // Navigation Properties
        public ICollection<Member> Members { get; set; } = new List<Member>();
        public Attendee? AttendeeInfo { get; set; }
    }
}
