using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbrella_Server.Data;
using Umbrella_Server.Data.Repositories.Groups;
using Umbrella_Server.DTOs.Group;
using Umbrella_Server.DTOs.Member;
using Umbrella_Server.Models;

namespace Umbrella_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupRepository _groupRepository;
        private readonly AppDbContext _context;

        public GroupController(AppDbContext context, IGroupRepository groupRepository)
        {
            _context = context;
            _groupRepository = groupRepository;
        }

        // ✅ GET: api/Group/{groupId}
        [HttpGet("{groupId}")]
        public async Task<ActionResult<GroupResponseDto>> GetGroup(Guid groupId)
        {
            var group = await _context.Groups
                                      .Include(g => g.Members)
                                      .ThenInclude(m => m.User)
                                      .FirstOrDefaultAsync(g => g.GroupID == groupId);

            if (group == null)
            {
                return NotFound(new { Message = $"Group with ID {groupId} not found." });
            }

            // Convert members to DTOs
            var memberDtos = group.Members.Select(m => new MemberDto
            {
                UserID = m.UserID,
                UserName = m.User?.Name ?? "Unknown",
                Roles = m.Roles.Select(r => r.ToString()).ToList()
            }).ToList();

            // Return group
            var responseDto = new GroupResponseDto
            {
                GroupID = group.GroupID,
                EventName = group.EventName,
                Description = group.Description,
                GroupLink = group.GroupLink,
                OrganizerID = group.OrganizerID,
                StartTime = group.StartTime,
                ExpireTime = group.ExpireTime,
                MeetingTime = group.MeetingTime,
                MeetingPlace = group.MeetingPlace,
                CreatedAt = group.CreatedAt,
                UpdatedAt = group.UpdatedAt,
                Members = memberDtos
            };

            return Ok(responseDto);
        }


        // ✅ POST: api/Group (Create Group)
        [HttpPost]
        public async Task<ActionResult<GroupResponseDto>> CreateGroup([FromBody] GroupCreateDto groupCreateDto)
        {
            // 🚀 Extract User ID from JWT (Commented out until Azure Auth is enabled)
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

            //Ensure the user is an Organizer
            if (!user.Roles.Contains(UserRole.Organizer))
            {
                user.Roles.Add(UserRole.Organizer);
                _context.Users.Update(user);
            }

            var group = new Group
            {
                EventName = groupCreateDto.EventName,
                Description = groupCreateDto.Description,
                OrganizerID = userId,
                StartTime = groupCreateDto.StartTime,
                ExpireTime = groupCreateDto.ExpireTime,
                MeetingTime = groupCreateDto.MeetingTime,
                MeetingPlace = groupCreateDto.MeetingPlace,
                GroupLink = Group.GenerateGroupCode(8)
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            //Add Organizer as a Member with Full Permissions
            var organizerMember = new Member
            {
                GroupID = group.GroupID,
                UserID = userId,
                Roles = new List<UserRole> { UserRole.Organizer, UserRole.Attendee },
                CanMessage = true, //Organizer can message
                CanCall = true,    //Organizer can call
                RsvpStatus = RsvpStatus.Accepted
            };

            //add and save organizer as a member
            _context.Members.Add(organizerMember);
            await _context.SaveChangesAsync();

            var responseDto = new GroupResponseDto
            {
                GroupID = group.GroupID,
                EventName = group.EventName,
                Description = group.Description,
                GroupLink = group.GroupLink,
                OrganizerID = userId,
                StartTime = group.StartTime,
                ExpireTime = group.ExpireTime,
                MeetingTime = group.MeetingTime,
                MeetingPlace = group.MeetingPlace,
                CreatedAt = group.CreatedAt,
                UpdatedAt = group.UpdatedAt
            };

            return CreatedAtAction(nameof(GetGroup), new { groupId = group.GroupID }, responseDto);
        }
    }
}
