namespace Umbrella_Server.DTOs.Member
{
    public class MemberDto
    {
        public Guid UserID { get; set; }
        public string? UserName { get; set; }
        public List<string> Roles { get; set; } = new List<string> { "Attendee" };
    }

}
