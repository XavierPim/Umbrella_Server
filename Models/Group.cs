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

        [Required(ErrorMessage = "Group link is required.")]
        public string GroupLink { get; set; } = string.Empty; // Group invite code (must be unique)

        [Required(ErrorMessage = "OrganizerID is required.")]
        public Guid OrganizerID { get; set; } // FK to User table

        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; } // When the event starts

        [Required(ErrorMessage = "Expiration time is required.")]
        public DateTime ExpireTime { get; set; } // When the event expires (auto-delete)

        [Required(ErrorMessage = "Meeting time is required.")]
        public DateTime MeetingTime { get; set; } // When participants are expected to meet

        [Required(ErrorMessage = "Meeting place is required.")]
        [MaxLength(300, ErrorMessage = "Meeting place cannot exceed 300 characters.")]
        public string MeetingPlace { get; set; } = string.Empty; // Address or link to Google Maps

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } // Automatically set when the group is created

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }// Automatically updated every time the group is modified

        // Navigation Properties
        public User? Organizer { get; set; } // Optional Organizer navigation property
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
