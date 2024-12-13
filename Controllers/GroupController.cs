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

        // GET: api/Group
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroups()
        {
            var groups = await _context.Groups
                .Include(g => g.Members)
                .ThenInclude(m => m.User)
                .ToListAsync();
            return Ok(groups);
        }

        // GET: api/Group/{groupId}
        [HttpGet("{groupId}")]
        public async Task<ActionResult<Group>> GetGroup(Guid groupId)
        {
            var group = await _context.Groups
                                      .Include(g => g.Members)
                                      .FirstOrDefaultAsync(g => g.GroupID == groupId);

            if (group == null)
            {
                return NotFound(new { Message = $"Group with ID {groupId} not found." });
            }

            return Ok(group);
        }

        // POST: api/Group
        [HttpPost]
        public async Task<ActionResult<Group>> CreateGroup(Group group)
        {
            group.GroupID = Guid.NewGuid();
            group.CreatedAt = DateTime.UtcNow;
            group.UpdatedAt = DateTime.UtcNow;

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGroup), new { groupId = group.GroupID }, group);
        }

        // PUT: api/Group/{groupId}
        [HttpPut("{groupId}")]
        public async Task<IActionResult> UpdateGroup(Guid groupId, Group updatedGroup)
        {
            if (groupId != updatedGroup.GroupID)
            {
                return BadRequest(new { Message = "Group ID in the URL does not match the body." });
            }

            _context.Entry(updatedGroup).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
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

        // ✅ POST: api/Group/{groupId}/members
        [HttpPost("{groupId}/members")]
        public async Task<ActionResult<Member>> AddMember(Guid groupId, [FromBody] Member member)
        {
            member.GroupID = groupId;

            // Check if user is already a member of the group
            var existingMember = await _context.Members
                .FirstOrDefaultAsync(m => m.GroupID == groupId && m.UserID == member.UserID);

            if (existingMember != null)
            {
                return Conflict(new { Message = "User is already a member of the group." });
            }

            member.Roles = (int)UserRole.Attendee; // Default role
            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMembers), new { groupId }, member);
        }

        // ✅ DELETE: api/Group/{groupId}/members/{userId}
        [HttpDelete("{groupId}/members/{userId}")]
        public async Task<IActionResult> RemoveMember(Guid groupId, Guid userId)
        {
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.GroupID == groupId && m.UserID == userId);

            if (member == null)
            {
                return NotFound(new { Message = "Member not found in the group." });
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ PUT: api/Group/{groupId}/members/{userId}/roles
        [HttpPut("{groupId}/members/{userId}/roles")]
        public async Task<IActionResult> UpdateMemberRoles(Guid groupId, Guid userId, [FromBody] UserRole[] roles)
        {
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.GroupID == groupId && m.UserID == userId);

            if (member == null)
            {
                return NotFound(new { Message = "Member not found in the group." });
            }

            // Reset roles, then add roles based on the input
            member.Roles = 0;
            foreach (var role in roles)
            {
                member.Roles |= (int)role; // Add role using bitwise OR
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Roles updated successfully.", CurrentRoles = member.Roles });
        }
    }
}
