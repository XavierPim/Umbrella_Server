using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Umbrella_Server.Models
{
    [Flags]
    public enum AdminPermissions
    {
        None = 0,             // 0000
        CanEdit = 1,          // 0001
        CanDelete = 2,        // 0010
        CanMakeAdmin = 4      // 0100
    }

    public class AdminUser
    {
        [Key]
        public Guid UserID { get; set; } // UserID serves as both the PK and FK to User

        [Required]
        public AdminPermissions Permissions { get; set; } = AdminPermissions.None; // Default permissions set to None

        // Navigation Properties (optional, could be used for Eager Loading)
        [ForeignKey("UserID")]
        public User? User { get; set; }
    }
}
