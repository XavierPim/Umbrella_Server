using System.ComponentModel.DataAnnotations;

public class GroupCreateDto
{
    public string EventName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid OrganizerID { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime ExpireTime { get; set; }
    public DateTime MeetingTime { get; set; }
    public string MeetingPlace { get; set; } = string.Empty;
}
