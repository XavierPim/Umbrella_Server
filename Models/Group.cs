using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Umbrella_Server.Models
{
    public class Group
    {
        [Key]
        public Guid GroupID { get; set; } = Guid.NewGuid(); // Unique identifier for the group

        [Required]
        public string EventName { get; set; } = string.Empty; // Name of the event

        [Required]
        public string Description { get; set; } = string.Empty; // Description of the event

        [Required]
        [Url(ErrorMessage = "GroupLink must be a valid URL.")]
        public string GroupLink { get; set; } = string.Empty; // Group invite URL or code (copiable and QR-friendly)

        [Required]
        [ForeignKey("Organizer")]
        public Guid OrganizerID { get; set; } // Foreign key to User table

        [Required]
        public DateTime StartTime { get; set; } // When the event starts

        [Required]
        public DateTime ExpireTime { get; set; } // When the event expires (auto-delete)

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } // Set using SQL Server's GETUTCDATE()

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } // Updated every time the group is modified

        // Navigation Properties
        [Required]
        public User Organizer { get; set; } = null!; // The user who created and manages the group

        public ICollection<Member> Members { get; set; } = new List<Member>();

        // Static utility for generating a default invite code (if needed)
        public static string GenerateGroupCode(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
