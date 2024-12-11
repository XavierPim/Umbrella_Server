using System.ComponentModel.DataAnnotations; 

namespace Umbrella_Server.Models
{
    public class Admin
    {
        [Key]
        public Guid AdminID { get; set; } // FK to UserID (AdminID = UserID)
        public int Permissions { get; set; } // Bitmask for permissions (1 = canEdit, 2 = canDelete, 4 = canMakeAdmin)

        // Navigation Properties
        public User User { get; set; } = null!; // Link back to User
    }
}
