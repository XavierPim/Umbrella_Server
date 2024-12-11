using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace Umbrella_Server.Models
{
    public class User
    {
        [Key]
        public Guid UserID { get; set; } = Guid.NewGuid(); // UUID for better security
        public int RoleID { get; set; } // 1 = Admin, 2 = Attendee, 3 = Organizer
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public string? Location { get; set; } // Using GEOGRAPHY type in SQL Server, store as WKT or lat/long
        public Guid GroupLink { get; set; } // FK to Group Table

        // Navigation Properties
        public ICollection<Member> Members { get; set; } = new List<Member>();
        public ICollection<Admin> Admins { get; set; } = new List<Admin>();
        public Attendee? AttendeeInfo { get; set; } // Optional link to attendee info
    }

}
