using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Umbrella_Server.Models
{
    
    public enum RsvpStatus
    {
        Pending = 0,
        Accepted = 1,
        Declined = 2
    }
    public class Member
    {
        [Required]
        public Guid GroupID { get; set; } // FK to Group Table

        [Required]
        public Guid UserID { get; set; } // FK to User Table

        // Store roles as a list of enum values (stored as a comma-separated string in the DB)
        [Required]
        public List<UserRole> Roles { get; set; } = new List<UserRole> { UserRole.Attendee }; // Default role is Attendee

        public bool CanMessage { get; set; } = false;
        public bool CanCall { get; set; } = false;

        [Required]
        public RsvpStatus RsvpStatus { get; set; } = RsvpStatus.Pending;

        // Navigation Properties 
        public Group? Group { get; set; } // Relationship to Group (nullable)
        public User? User { get; set; } // Relationship to User (nullable)
    }
}
