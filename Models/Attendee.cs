using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Umbrella_Server.Models
{
    public enum RsvpStatus
    {
        Pending = 0,
        Accepted = 1,
        Declined = 2
    }

    public class Attendee
    {
        [Key]
        public Guid UserID { get; set; } // Primary key and FK to User table

        [Required]
        public User User { get; set; } = null!; // Required link to the User

        [Required]
        public bool CanMessage { get; set; } = false; // Default is false

        [Required]
        public bool CanCall { get; set; } = false; // Default is false

        [Required]
        public RsvpStatus RsvpStatus { get; set; } = RsvpStatus.Pending; // Default is Pending
    }
}
