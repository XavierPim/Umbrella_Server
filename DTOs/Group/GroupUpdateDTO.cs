using System.ComponentModel.DataAnnotations;

namespace Umbrella_Server.DTOs.Group
{
    public class GroupUpdateDto
    {
        [Required(ErrorMessage = "Event name is required.")]
        [MaxLength(100, ErrorMessage = "Event name cannot exceed 100 characters.")]
        public string? EventName { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? ExpireTime { get; set; }
        public DateTime? MeetingTime { get; set; }
        public string? MeetingPlace { get; set; }
    }
}
