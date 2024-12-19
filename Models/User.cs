using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Umbrella_Server.Models
{
    public enum UserRole
    {
        Admin,
        Attendee,
        Organizer
    }

    public class User
    {
        [Key]
        public Guid UserID { get; set; } = Guid.NewGuid();

        [Required]
        public required string Name { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }

        // 🔥 Store roles as a single string in the database, split on comma
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Roles { get; set; } = UserRole.Attendee.ToString(); // Store as a comma-separated string

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [JsonIgnore]
        public string GroupLink { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<Member> Members { get; set; } = new List<Member>();

        [JsonIgnore]
        public Attendee? AttendeeInfo { get; set; }

        [JsonIgnore]
        public AdminUser? AdminInfo { get; set; }

        // 🔥 Helper method to convert the comma-separated Roles into a List<UserRole>
        public List<UserRole> GetRoles()
        {
            return Roles.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(role => Enum.Parse<UserRole>(role))
                        .ToList();
        }

        // 🔥 Helper method to set roles from a List<UserRole>
        public void SetRoles(List<UserRole> roles)
        {
            Roles = string.Join(',', roles.Select(r => r.ToString()));
        }
    }
}
