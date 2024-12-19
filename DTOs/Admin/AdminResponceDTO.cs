namespace Umbrella_Server.DTOs.Admin
{
    public class AdminResponseDto
    {
        public Guid UserID { get; set; }
        public List<string> Permissions { get; set; } = new List<string>(); // List of permission names (CanEdit, CanDelete, etc.)
    }
}
