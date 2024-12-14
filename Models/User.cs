using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Umbrella_Server.Models;

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

    [Required]
    public List<UserRole> Roles { get; set; } = new List<UserRole> { UserRole.Attendee };

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public string GroupLink { get; set; } = string.Empty;

    // Navigation Properties
    public ICollection<Member> Members { get; set; } = new List<Member>();
    public Attendee? AttendeeInfo { get; set; }
    public Admin? AdminInfo { get; set; }
}
