using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Umbrella_Server.Data;
using Umbrella_Server.DTOs.Group;
using Umbrella_Server.DTOs.Member;
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

            // ✅ Return GroupResponseDto instead of raw model
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
                UpdatedAt = group.UpdatedAt
            };

            return Ok(responseDto);
        }

        // ✅ POST: api/Group (Create Group)
        [HttpPost]
        public async Task<ActionResult<GroupResponseDto>> CreateGroup([FromBody] GroupCreateDto groupCreateDto)
        {
            if (groupCreateDto.OrganizerID == Guid.Empty)
            {
                return BadRequest(new { Message = "OrganizerID is required." });
            }

            var userExists = await _context.Users.AnyAsync(u => u.UserID == groupCreateDto.OrganizerID);
            if (!userExists)
            {
                return NotFound(new { Message = $"User with ID {groupCreateDto.OrganizerID} does not exist." });
            }

            var group = new Group
            {
                EventName = groupCreateDto.EventName,
                Description = groupCreateDto.Description,
                OrganizerID = groupCreateDto.OrganizerID,
                StartTime = groupCreateDto.StartTime,
                ExpireTime = groupCreateDto.ExpireTime,
                MeetingTime = groupCreateDto.MeetingTime,
                MeetingPlace = groupCreateDto.MeetingPlace,
                GroupLink = Group.GenerateGroupCode(8) // Generate GroupLink automatically
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

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
                UpdatedAt = group.UpdatedAt
            };

            return CreatedAtAction(nameof(GetGroup), new { groupId = group.GroupID }, responseDto);
        }

        // ✅ GET: api/Group/{groupId}/members
        [HttpGet("{groupId}/members")]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetMembers(Guid groupId)
        {
            var members = await _context.Members
                .Where(m => m.GroupID == groupId)
                .Include(m => m.User)
                .Select(m => new MemberDto
                {
                    UserID = m.UserID,
                    UserName = m.User.Name,
                    Roles = m.Roles.Select(r => r.ToString()).ToList()
                })
                .ToListAsync();

            return Ok(members);
        }

        // ✅ PUT: api/Group/{groupId}/members
        [HttpPut("{groupId}/members")]
        public async Task<ActionResult> UpdateGroupMembers(Guid groupId, [FromBody] List<MemberDto> members)
        {
            if (members == null || !members.Any())
            {
                return BadRequest(new { Message = "Member list cannot be empty." });
            }

            var group = await _context.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.GroupID == groupId);
            if (group == null)
            {
                return NotFound(new { Message = $"Group with ID {groupId} not found." });
            }

            var userIds = members.Select(m => m.UserID).Distinct().ToList();
            var existingUsers = await _context.Users.Where(u => userIds.Contains(u.UserID)).Select(u => u.UserID).ToListAsync();

            var missingUsers = userIds.Except(existingUsers).ToList();
            if (missingUsers.Any())
            {
                return NotFound(new { Message = "Some users were not found.", MissingUserIds = missingUsers });
            }

            var existingMemberIds = group.Members.Select(m => m.UserID).ToList();

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
                        Roles = memberDto.Roles.Select(r => Enum.Parse<UserRole>(r)).ToList()
                    };
                    _context.Members.Add(newMember);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Members updated successfully." });
        }
    }
}
