namespace Umbrella_Server.DTOs.User
{
    public class UserResponseDto
    {
        public Guid UserID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
