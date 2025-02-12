using System.ComponentModel.DataAnnotations;

namespace Umbrella_Server.DTOs.ActiveGroup
{
    public class LocationUpdateDto
    {
        public Guid UserID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}