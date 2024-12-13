using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Umbrella_Server.Models
{
    public class Member
    {
        [Required]
        [ForeignKey("Group")]
        public Guid GroupID { get; set; } // FK to Group Table

        [Required]
        [ForeignKey("User")]
        public Guid UserID { get; set; } // FK to User Table

        [Required]
        public required UserRole RoleID { get; set; } 

        // Navigation Properties
        [Required]
        public Group Group { get; set; } = null!; // Relationship to Group

        [Required]
        public User User { get; set; } = null!; // Relationship to User
    }
}
