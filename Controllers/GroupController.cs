using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbrella_Server.Data;
using Umbrella_Server.Models;

namespace Umbrella_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/Group/{groupId}
        [HttpGet("{groupId}")]
        public async Task<ActionResult<Group>> GetGroup(Guid groupId)
        {
            var group = await _context.Groups
                                      .Include(g => g.Members)
                                      .ThenInclude(m => m.User) // Include user details for each member
                                      .FirstOrDefaultAsync(g => g.GroupID == groupId);

            if (group == null)
            {
                return NotFound(new { Message = $"Group with ID {groupId} not found." });
            }

            return Ok(group);
        }

        // ✅ POST: api/Group (Create Group)
        //[HttpPost]
        //public async Task<ActionResult<Group>> CreateGroup(Group group)
        //{
        //    // 1️⃣ Validate the Organizer exists
        //    //var organizer = await _context.Users.FindAsync(group.OrganizerID);
        //    //if (organizer == null)
        //    //{
        //    //    return NotFound(new { Message = $"Organizer with ID {group.OrganizerID} not found." });
        //    //}

        //    // 2️⃣ Set default values and create GroupLink if not set
        //    group.GroupID = Guid.NewGuid();
        //    group.CreatedAt = DateTime.UtcNow;
        //    group.UpdatedAt = DateTime.UtcNow;

        //    // Check and ensure GroupLink is set, if not, generate it
        //    if (string.IsNullOrWhiteSpace(group.GroupLink))
        //    {
        //        group.GroupLink = Group.GenerateGroupCode(8); // Generate an 8-character GroupLink
        //    }

        //    //group.Organizer = organizer; // 👈 Attach User object to Organizer navigation property

        //    // 3️⃣ Add the Group to the database
        //    _context.Groups.Add(group);
        //    await _context.SaveChangesAsync();

        //    // 4️⃣ Add the Organizer as the first Member of the group with Organizer and Attendee roles
        //    //var organizerMember = new Member
        //    //{
        //    //    GroupID = group.GroupID,
        //    //    UserID = group.OrganizerID, // Organizer becomes the first member
        //    //    Roles = (int)UserRole.Organizer | (int)UserRole.Attendee // Organizer and Attendee roles
        //    //};

        //    //_context.Members.Add(organizerMember);
        //    await _context.SaveChangesAsync();

        //    // 5️⃣ Return the newly created group with the "GetGroup" action
        //    return CreatedAtAction(nameof(GetGroup), new { groupId = group.GroupID }, group);
        //}`

        // ✅ POST: api/Group (Create Group)
        [HttpPost]
        public async Task<ActionResult<Group>> CreateGroup([FromBody] Group group)
        {
            // 1️⃣ Validate the Organizer exists
            var organizer = await _context.Users.FindAsync(group.OrganizerID);
            if (organizer == null)
            {
                return NotFound(new { Message = $"User with ID {group.OrganizerID} not found." });
            }

            // 2️⃣ Set default values for Group
            group.GroupID = Guid.NewGuid();

            // ❌ No need to manually set CreatedAt or UpdatedAt
            // group.CreatedAt = DateTime.UtcNow; 
            // group.UpdatedAt = DateTime.UtcNow; 

            // Add the group to the database
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            // Add Organizer as a member in the group
            var organizerMember = new Member
            {
                GroupID = group.GroupID,
                UserID = group.OrganizerID,
                Roles = (int)UserRole.Organizer | (int)UserRole.Attendee
            };

            _context.Members.Add(organizerMember);
            await _context.SaveChangesAsync();

            // Return the newly created group with the "GetGroup" action
            return CreatedAtAction(nameof(GetGroup), new { groupId = group.GroupID }, group);
        }

        // ✅ GET: api/Group/{groupId}/members
        [HttpGet("{groupId}/members")]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers(Guid groupId)
        {
            var members = await _context.Members
                .Where(m => m.GroupID == groupId)
                .Include(m => m.User)
                .ToListAsync();

            return Ok(members);
        }

        // ✅ POST: api/Group/{groupId}/members (Batch create members)
        [HttpPost("{groupId}/members")]
        public async Task<ActionResult<IEnumerable<Member>>> AddMembers(Guid groupId, [FromBody] List<Member> members)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                return NotFound(new { Message = $"Group with ID {groupId} not found." });
            }

            var addedMembers = new List<Member>();

            foreach (var member in members)
            {
                member.GroupID = groupId;

                // Check if the user is already a member of the group
                var existingMember = await _context.Members
                    .FirstOrDefaultAsync(m => m.GroupID == groupId && m.UserID == member.UserID);

                if (existingMember != null)
                {
                    continue; // Skip users that are already members
                }

                // Default role is Attendee unless otherwise specified
                if (member.Roles == 0)
                {
                    member.Roles = (int)UserRole.Attendee;
                }

                // Add to context
                _context.Members.Add(member);
                addedMembers.Add(member);
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Members added successfully.", Members = addedMembers });
        }
    }
}
