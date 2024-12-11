using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace Umbrella_Server.Models
{
    public class Attendee
    {
        [Key]
        public Guid UserID { get; set; } // FK to User Table
        public bool CanMessage { get; set; } = false;
        public bool CanCall { get; set; } = false;
        public string RsvpStatus { get; set; } = "Pending"; // Enum-like value (e.g., Pending, Going, Not Going)

        // Navigation Properties
        public User User { get; set; } = null!; // Link back to User
    }
}
