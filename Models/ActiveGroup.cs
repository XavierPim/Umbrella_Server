using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Umbrella_Server.Models
{
    public class ActiveGroup
    {
        [Required]
        public Guid GroupID { get; set; } // FK to Group

        [Required]
        public Guid UserID { get; set; } // FK to User

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        public float? DistanceFromOrganizer { get; set; } // Nullable distance precomputed for efficiency

        // Navigation Properties
        public User User { get; set; } = null!;
    }
}
