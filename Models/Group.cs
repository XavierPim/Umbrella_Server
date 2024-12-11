using Microsoft.EntityFrameworkCore;
namespace Umbrella_Server.Models
{
    public class Group
    {
        public Guid GroupID { get; set; } = Guid.NewGuid(); // UUID for better security
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid GroupLink { get; set; } = Guid.NewGuid(); // Used to join groups, unique and hard to guess
        public Guid OrganizerID { get; set; } // FK to User Table
        public DateTime StartTime { get; set; }
        public DateTime ExpireTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public User Organizer { get; set; } = null!; // The user who organized the group
        public ICollection<Member> Members { get; set; } = new List<Member>();
    }
}
