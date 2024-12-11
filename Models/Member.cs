
using System.ComponentModel.DataAnnotations;

namespace Umbrella_Server.Models
{
    public class Member
    {
        [Key]
        public Guid MemberID { get; set; } = Guid.NewGuid(); // Primary Key
        public Guid GroupID { get; set; } // FK to Group Table
        public Guid UserID { get; set; } // FK to User Table
        public int RoleID { get; set; } // 1 = Admin, 2 = Attendee, 3 = Organizer

        // Navigation Properties
        public Group Group { get; set; } = null!;
        public User User { get; set; } = null!;
    }

}
