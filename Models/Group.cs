using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Umbrella_Server.Models
{
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid GroupID { get; set; }

        [Required(ErrorMessage = "Event name is required.")]
        [MaxLength(100, ErrorMessage = "Event name cannot exceed 100 characters.")]
        public required string EventName { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Group link is required.")]
        [MinLength(8, ErrorMessage = "Group link must be at least 8 characters.")]
        public string GroupLink { get; set; } = string.Empty;

        [Required(ErrorMessage = "OrganizerID is required.")]
        public Guid OrganizerID { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Expiration time is required.")]
        public DateTime ExpireTime { get; set; }

        [Required(ErrorMessage = "Meeting time is required.")]
        public DateTime MeetingTime { get; set; }

        [Required(ErrorMessage = "Meeting place is required.")]
        [MaxLength(300, ErrorMessage = "Meeting place cannot exceed 300 characters.")]
        public required string MeetingPlace { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
  
        public User? Organizer { get; set; }
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
