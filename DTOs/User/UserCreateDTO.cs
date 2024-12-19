using System.ComponentModel.DataAnnotations;

namespace Umbrella_Server.DTOs.User
{
    public class UserCreateDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name must be less than 50 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;
    }
}
