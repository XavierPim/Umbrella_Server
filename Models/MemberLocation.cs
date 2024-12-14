//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Umbrella_Server.Models
//{
//    public class MemberLocation
//    {
//        [Required]
//        public Guid UserID { get; set; }

//        [Required]
//        public Guid GroupID { get; set; }

//        [Required]
//        public double Latitude { get; set; } 

//        [Required]
//        public double Longitude { get; set; } 

//        [Required]
//        public DateTime TimeStamp { get; set; }

//        public float? DistanceFromOrganizer { get; set; }

//        // Navigation Properties
//        public User User { get; set; } = null!;
//        public Group Group { get; set; } = null!;
//    }
//}
