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
        public Guid UserID { get; set; }

        [Required]
        [ForeignKey("UserID")]
        public User User { get; set; } = null!; // Ensures User link is required

        [Required]
        [DefaultValue(false)]
        public bool CanMessage { get; set; } = false;

        [Required]
        [DefaultValue(false)]
        public bool CanCall { get; set; } = false;

        [Required]
        public RsvpStatus RsvpStatus { get; set; } = RsvpStatus.Pending; // Store as "Pending", "Accepted", "Declined"
    }
}
