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
    // [Authorize] // 🔐 Require authentication for all member actions
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

            // var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // if (userIdClaim == null) return Unauthorized();
            // var userId = Guid.Parse(userIdClaim);

            // 🚀 For Development: Use a Hardcoded User ID Until Azure Auth is Integrated
            var userId = Guid.Parse("ddf28569-ead9-49ba-a1e0-78e73fd261a8"); // Replace when JWT is active

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = $"User with ID {userId} does not exist." });
            }

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
        // Add a member to a group (only organizers can do this)
        [HttpPost("{groupId}")]
        public async Task<ActionResult> AddMember(Guid groupId, [FromBody] MemberDto newMemberDto)
        {
            var group = await _context.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.GroupID == groupId);
            if (group == null)
            {
                return NotFound(new { Message = "Group not found." });
            }

            // 🔐 Future JWT Extraction Placeholder (Currently Hardcoded)
            // var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // if (userIdClaim == null) return Unauthorized();
            // var userId = Guid.Parse(userIdClaim);

            // 🔹 For Development: Use a Hardcoded User ID Until JWT is Integrated
            var userId = Guid.Parse("ddf28569-ead9-49ba-a1e0-78e73fd261a8"); // Replace when JWT is active

            // ✅ Check if the requester is the organizer
            if (group.OrganizerID != userId)
            {
                return StatusCode(403, new { Message = "Only the organizer can add members to this group." });
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
                CanMessage = true,  // ✅ Default: Members can message
                CanCall = false,    // ❌ Default: Members cannot call
                RsvpStatus = RsvpStatus.Pending
            };

            _context.Members.Add(newMember);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Member added successfully." });
        }

        // ✅ PUT: api/Member/{groupId}
        // Organizer updates multiple members
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

            // 🔐 Future JWT Extraction Placeholder (Currently Hardcoded)
            // var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // if (userIdClaim == null) return Unauthorized();
            // var userId = Guid.Parse(userIdClaim);

            // 🔹 Hardcoded User ID Until JWT is Enabled
            var userId = Guid.Parse("67abb0cf-8ec6-4d90-9009-75430147df2d"); // Replace when JWT is active

            // ✅ Only the organizer can update members
            if (group.OrganizerID != userId)
            {
                return StatusCode(403, new { Message = "Only the organizer can update group members." });
            }

            foreach (var memberDto in members)
            {
                var existingMember = group.Members.FirstOrDefault(m => m.UserID == memberDto.UserID);

                if (existingMember != null)
                {
                    // 🔹 Prevent Changing Organizer Role
                    if (existingMember.UserID == group.OrganizerID && !memberDto.Roles.Contains("Organizer"))
                    {
                        return BadRequest(new { Message = "The organizer's role cannot be removed or modified." });
                    }

                    // ✅ Update existing member roles
                    existingMember.Roles = memberDto.Roles.Select(r => Enum.Parse<UserRole>(r)).ToList();
                }
                else
                {
                    // ✅ Skip adding a member if they already joined via link
                    if (_context.Members.Any(m => m.GroupID == groupId && m.UserID == memberDto.UserID))
                    {
                        continue;
                    }

                    // ✅ Add new member
                    var newMember = new Member
                    {
                        GroupID = groupId,
                        UserID = memberDto.UserID,
                        Roles = memberDto.Roles.Select(r => Enum.Parse<UserRole>(r)).ToList(),
                        CanMessage = true,
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

        // ✅ GET: api/Member/{groupId}/link
        // Retrieves the shareable group link for members
        [HttpGet("{groupId}/link")]
        public async Task<ActionResult<object>> GetGroupLink(Guid groupId)
        {
            var group = await _context.Groups
                .Where(g => g.GroupID == groupId)
                .Select(g => new { g.GroupLink })
                .FirstOrDefaultAsync();

            if (group == null)
            {
                return NotFound(new { Message = "Group not found." });
            }

            return Ok(new { ShareableLink = $"https://umbrellaapp.com/join/{group.GroupLink}" });
        }


        // ✅ POST: api/Member/join/{groupLink}
        // Allows users to join a group using the invite link
        [HttpPost("join/{groupLink}")]
        public async Task<ActionResult> JoinGroup(string groupLink)
        {
            var group = await _context.Groups
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.GroupLink == groupLink);

            if (group == null)
            {
                return NotFound(new { Message = "Invalid or expired invite link." });
            }

            // 🔐 Future JWT Extraction Placeholder (Currently Hardcoded)
            // var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // if (userIdClaim == null) return Unauthorized();
            // var userId = Guid.Parse(userIdClaim);

            // 🔹 Hardcoded User ID Until JWT is Active
            var userId = Guid.Parse("6eb8cf56-1ca7-46d4-ae60-2c4d778d6459"); // Replace with JWT user ID

            // ✅ Check if the user is already in the group
            if (group.Members.Any(m => m.UserID == userId))
            {
                return BadRequest(new { Message = "You are already a member of this group." });
            }

            // ✅ Add User as an Attendee
            var newMember = new Member
            {
                GroupID = group.GroupID,
                UserID = userId,
                Roles = new List<UserRole> { UserRole.Attendee }, // Default role for joining members
                CanMessage = true,
                CanCall = false,
                RsvpStatus = RsvpStatus.Pending
            };

            _context.Members.Add(newMember);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Successfully joined the group!" });
        }



        // ✅ DELETE: api/Member/{groupId}/{userId}
        // Remove a member from a group (only organizer can remove others, but users can leave themselves)
        [HttpDelete("{groupId}/{userId}")]
        public async Task<IActionResult> RemoveMember(Guid groupId, Guid userId)
        {
            var group = await _context.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.GroupID == groupId);
            if (group == null)
            {
                return NotFound(new { Message = "Group not found." });
            }

            var member = await _context.Members.FirstOrDefaultAsync(m => m.GroupID == groupId && m.UserID == userId);
            if (member == null)
            {
                return NotFound(new { Message = "Member not found." });
            }

            // 🔐 Future JWT Extraction Placeholder (Currently Hardcoded)
            // var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // if (userIdClaim == null) return Unauthorized();
            // var requesterId = Guid.Parse(userIdClaim);

            // 🔹 Hardcoded User ID Until JWT is Enabled
            var requesterId = Guid.Parse("ddf28569-ead9-49ba-a1e0-78e73fd261a8"); // Replace when JWT is active

            // 🔹 Case 1: Prevent Removing the Organizer
            if (member.UserID == group.OrganizerID)
            {
                return BadRequest(new { Message = "The organizer cannot be removed from the group." });
            }

            // 🔹 Case 2: Allow Users to Remove Themselves (Leave Group)
            if (requesterId == userId)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "You have left the group." });
            }

            // 🔹 Case 3: Only Organizer Can Remove Others
            if (requesterId != group.OrganizerID)
            {
                return StatusCode(403, new { Message = "Only the organizer can remove other members." });
            }

            // ✅ Remove Member (Organizer Removing Someone Else)
            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Member removed from group." });
        }
    }
}
