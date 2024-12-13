using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Umbrella_Server.Models
{
    public class Group
    {
        [Key]
        public Guid GroupID { get; set; } = Guid.NewGuid(); // Unique identifier for the group

        [Required(ErrorMessage = "Event name is required.")]
        [MaxLength(100, ErrorMessage = "Event name cannot exceed 100 characters.")]
        public string EventName { get; set; } = string.Empty; // Name of the event

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty; // Description of the event

        [Required(ErrorMessage = "GroupLink is required.")]
        public string GroupLink { get; set; } = string.Empty; // Group invite code (QR-friendly)

        [Required(ErrorMessage = "OrganizerID is required.")]
        [ForeignKey(nameof(Organizer))]
        public Guid OrganizerID { get; set; } // FK to User table

        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; } // When the event starts

        [Required(ErrorMessage = "Expiration time is required.")]
        public DateTime ExpireTime { get; set; } // When the event expires (auto-delete)

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Automatically set when the group is created

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Automatically updated every time the group is modified

        // Navigation Properties
        public User Organizer { get; set; } = null!; // The user who created and manages the group

        public ICollection<Member> Members { get; set; } = new List<Member>();

        public void EnsureGroupLink(int length = 8)
        {
            if (string.IsNullOrEmpty(GroupLink))
            {
                GroupLink = GenerateGroupCode(length);
            }
        }
        public static string GenerateGroupCode(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
