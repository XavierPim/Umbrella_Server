using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Umbrella_Server.Models
{
    public class User
    {
        [Key]
        public Guid UserID { get; set; } = Guid.NewGuid();

        [Required]
        public required UserRole RoleID { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Email { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }

        // Replace Point with Latitude and Longitude (simple doubles)
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string GroupLink { get; set; } = string.Empty; // Invite code (e.g., ABC123)

        // Navigation Properties
        public ICollection<Member> Members { get; set; } = new List<Member>();

        public Attendee? AttendeeInfo { get; set; }
    }
}
