using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbrella_Server.Data;
using Umbrella_Server.DTOs.Member;
using Umbrella_Server.Models;

namespace Umbrella_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 🔐 Require authentication for all member actions
    public class MemberController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MemberController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/Member/{groupId}
        // Get all members in a group
        [HttpGet("{groupId}")]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetGroupMembers(Guid groupId)
        {
            var members = await _context.Members
                .Where(m => m.GroupID == groupId)
                .Include(m => m.User)
                .Select(m => new MemberDto
                {
                    UserID = m.UserID,
                    UserName = m.User != null ? m.User.Name : "Unknown",
                    Roles = m.Roles.Select(r => r.ToString()).ToList(),
                    CanMessage = m.CanMessage,
                    CanCall = m.CanCall,
                    RsvpStatus = m.RsvpStatus
                })
                .ToListAsync();

            return Ok(members);
        }

        // ✅ POST: api/Member/{groupId}
        // Add a member to a group
        [HttpPost("{groupId}")]
        public async Task<ActionResult> AddMember(Guid groupId, [FromBody] MemberDto newMemberDto)
        {
            var group = await _context.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.GroupID == groupId);
            if (group == null)
            {
                return NotFound(new { Message = "Group not found." });
            }

            var user = await _context.Users.FindAsync(newMemberDto.UserID);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            // ✅ Check if the user is already a member
            if (group.Members.Any(m => m.UserID == newMemberDto.UserID))
            {
                return BadRequest(new { Message = "User is already a member of this group." });
            }

            var newMember = new Member
            {
                GroupID = groupId,
                UserID = newMemberDto.UserID,
                Roles = newMemberDto.Roles.Select(r => Enum.Parse<UserRole>(r)).ToList(),
                CanMessage = true, // ✅ Default: Members can message
                CanCall = false,   // ❌ Default: Members cannot call
                RsvpStatus = RsvpStatus.Pending
            };

            _context.Members.Add(newMember);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Member added successfully." });
        }

        // ✅ PUT: api/Member/{groupId}
        // Update multiple members in a group
        [HttpPut("{groupId}")]
        public async Task<ActionResult> UpdateGroupMembers(Guid groupId, [FromBody] List<MemberDto> members)
        {
            if (members == null || !members.Any())
            {
                return BadRequest(new { Message = "Member list cannot be empty." });
            }

            var group = await _context.Groups
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.GroupID == groupId);

            if (group == null)
            {
                return NotFound(new { Message = $"Group with ID {groupId} not found." });
            }

            var userIds = members.Select(m => m.UserID).Distinct().ToList();
            var existingUsers = await _context.Users
                .Where(u => userIds.Contains(u.UserID))
                .Select(u => u.UserID)
                .ToListAsync();

            var missingUsers = userIds.Except(existingUsers).ToList();
            if (missingUsers.Any())
            {
                return NotFound(new { Message = "Some users were not found.", MissingUserIds = missingUsers });
            }

            foreach (var memberDto in members)
            {
                var existingMember = group.Members.FirstOrDefault(m => m.UserID == memberDto.UserID);
                if (existingMember != null)
                {
                    existingMember.Roles = memberDto.Roles.Select(r => Enum.Parse<UserRole>(r)).ToList();
                }
                else
                {
                    var newMember = new Member
                    {
                        GroupID = groupId,
                        UserID = memberDto.UserID,
                        Roles = memberDto.Roles.Select(r => Enum.Parse<UserRole>(r)).ToList(),
                        CanMessage = true, // ✅ Default for new members
                        CanCall = false
                    };
                    _context.Members.Add(newMember);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Members updated successfully." });
        }

        // ✅ PUT: api/Member/{groupId}/{userId}/permissions
        // Update member permissions (CanMessage, CanCall, RSVP)
        [HttpPut("{groupId}/{userId}/permissions")]
        public async Task<IActionResult> UpdateMemberPermissions(Guid groupId, Guid userId, [FromBody] MemberPermissionsDto permissionsDto)
        {
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.GroupID == groupId && m.UserID == userId);

            if (member == null)
            {
                return NotFound(new { Message = "Member not found in this group." });
            }

            // ✅ Update permissions
            member.CanMessage = permissionsDto.CanMessage;
            member.CanCall = permissionsDto.CanCall;
            member.RsvpStatus = permissionsDto.RsvpStatus;

            _context.Members.Update(member);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Member permissions updated successfully." });
        }

        // ✅ DELETE: api/Member/{groupId}/{userId}
        // Remove a member from a group
        [HttpDelete("{groupId}/{userId}")]
        public async Task<IActionResult> RemoveMember(Guid groupId, Guid userId)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.GroupID == groupId && m.UserID == userId);
            if (member == null)
            {
                return NotFound(new { Message = "Member not found." });
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Member removed from group." });
        }
    }
}
