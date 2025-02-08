using Umbrella_Server.Models;
namespace Umbrella_Server.DTOs.Member
{
    public class MemberDto
    {
        public Guid UserID { get; set; }
        public string? UserName { get; set; }
           public bool CanMessage { get; set; } = true; 
        public bool CanCall { get; set; } = false; 
        public RsvpStatus RsvpStatus { get; set; } = RsvpStatus.Pending;
        public List<string> Roles { get; set; } = new List<string> { "Attendee" };
    }

}
