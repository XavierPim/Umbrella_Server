namespace Umbrella_Server.Models
{
    [Flags]
    public enum UserRole
    {
        None = 0,        // No role
        Admin = 1 << 0,  // 1 (binary 0001)
        Attendee = 1 << 1, // 2 (binary 0010)
        Organizer = 1 << 2  // 4 (binary 0100)
                           
    }

}
