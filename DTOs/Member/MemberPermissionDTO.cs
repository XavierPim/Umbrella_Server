using Umbrella_Server.Models;
namespace Umbrella_Server.DTOs.Member

{
    public class MemberPermissionsDto
    {
        public bool CanMessage { get; set; }
        public bool CanCall { get; set; }
        public RsvpStatus RsvpStatus { get; set; }
    }
}