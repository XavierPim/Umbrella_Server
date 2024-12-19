using System.ComponentModel.DataAnnotations;

namespace Umbrella_Server.DTOs
{
    public class UserCreateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty; 
    }

}
