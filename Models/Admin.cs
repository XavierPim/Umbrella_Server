using System.ComponentModel;
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

    public class Admin
    {
        [Key]
        [ForeignKey("User")]
        public Guid UserID { get; set; } // UserID serves as both the PK and FK to User

        [Required]
        [ForeignKey("UserID")]
        public User User { get; set; } = null!; // Required link to the User (master admin)

        [Required]
        [DefaultValue(AdminPermissions.None)]
        public AdminPermissions Permissions { get; set; } = AdminPermissions.None; // Bitmask for permissions (1 = CanEdit, 2 = CanDelete, 4 = CanMakeAdmin)

        // Helper method to check if the Admin has a specific permission
        public bool HasPermission(AdminPermissions permission)
        {
            return (Permissions & permission) == permission;
        }
    }
}
