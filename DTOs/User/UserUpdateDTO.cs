using System.ComponentModel.DataAnnotations;
using Umbrella_Server.Models;

namespace Umbrella_Server.DTOs.User
{
    public class UserUpdateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public List<UserRole>? Roles { get; set; } 
    }
}
