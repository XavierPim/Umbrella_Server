using System.ComponentModel.DataAnnotations;
using Umbrella_Server.Models;

namespace Umbrella_Server.DTOs.User
{
    public class UserCreateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public List<UserRole> Roles { get; set; } = new List<UserRole> { UserRole.Attendee };
    }
}
