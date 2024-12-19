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
                                      .ThenInclude(m => m.User)
                                      .FirstOrDefaultAsync(g => g.GroupID == groupId);

            if (group == null)
            {
                return NotFound(new { Message = $"Group with ID {groupId} not found." });
            }

            return Ok(group);
        }

        // ✅ POST: api/Group (Create Group)
        [HttpPost]
        public async Task<ActionResult> CreateGroup([FromBody] Group groupRequest)
        {
            // ✅ Check if OrganizerID is present
            if (groupRequest.OrganizerID == Guid.Empty)
            {
                return BadRequest(new { Message = "OrganizerID is required." });
            }

            // ✅ Check if the user exists
            var userExists = await _context.Users.AnyAsync(u => u.UserID == groupRequest.OrganizerID);
            if (!userExists)
            {
                return NotFound(new
                {
                    Message = $"User with ID {groupRequest.OrganizerID} does not exist."
                });
            }

            // ✅ Manually create the Group object (to avoid automatic validation errors)
            var group = new Group
            {
                GroupID = Guid.NewGuid(),
                EventName = groupRequest.EventName,
                Description = groupRequest.Description,
                OrganizerID = groupRequest.OrganizerID,
                StartTime = groupRequest.StartTime,
                ExpireTime = groupRequest.ExpireTime,
                MeetingTime = groupRequest.MeetingTime,
                MeetingPlace = groupRequest.MeetingPlace,
            };

            // ✅ Generate a GroupLink if one is not provided
            if (string.IsNullOrWhiteSpace(groupRequest.GroupLink))
            {
                group.GroupLink = Group.GenerateGroupCode(8); // Generate a random 8-character GroupLink
            }
            else
            {
                group.GroupLink = groupRequest.GroupLink;
            }

            // ✅ Save the group to the database
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            // ✅ Return the whole Group object
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

        // ✅ PUT: api/Group/{groupId}/members (Add or Update members in a group)
        [HttpPut("{groupId}/members")]
        public async Task<ActionResult<IEnumerable<Member>>> UpdateGroupMembers(Guid groupId, [FromBody] List<Member> members)
        {
            if (members == null || !members.Any())
            {
                return BadRequest(new { Message = "Member list cannot be empty." });
            }

            // ✅ Check if the group exists
            var group = await _context.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.GroupID == groupId);
            if (group == null)
            {
                return NotFound(new { Message = $"Group with ID {groupId} not found." });
            }

            // ✅ Get all UserIDs from the request to avoid querying the DB in the loop
            var userIdsInRequest = members.Select(m => m.UserID).Distinct().ToList();

            // ✅ Check for invalid users
            var existingUserIds = await _context.Users
                .Where(u => userIdsInRequest.Contains(u.UserID))
                .Select(u => u.UserID)
                .ToListAsync();

            var missingUsers = userIdsInRequest.Except(existingUserIds).ToList();
            if (missingUsers.Any())
            {
                return NotFound(new
                {
                    Message = "Some users were not found.",
                    MissingUserIds = missingUsers
                });
            }

            // ✅ Get existing members for the group
            var existingMemberIds = await _context.Members
                .Where(m => m.GroupID == groupId)
                .Select(m => m.UserID)
                .ToListAsync();

            var addedOrUpdatedMembers = new List<Member>();

            foreach (var member in members)
            {
                if (existingMemberIds.Contains(member.UserID))
                {
                    // ✅ Update existing member's roles
                    var existingMember = await _context.Members
                        .FirstOrDefaultAsync(m => m.GroupID == groupId && m.UserID == member.UserID);

                    if (existingMember != null)
                    {
                        // ✅ Update Roles (overwrite roles with the new set)
                        existingMember.Roles = member.Roles ?? new List<UserRole> { UserRole.Attendee };
                    }
                }
                else
                {
                    // ✅ Create new Member object
                    var newMember = new Member
                    {
                        GroupID = groupId,
                        UserID = member.UserID,
                        Roles = member.Roles ?? new List<UserRole> { UserRole.Attendee }
                    };

                    _context.Members.Add(newMember);
                    addedOrUpdatedMembers.Add(newMember);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Members updated successfully.",
                Members = addedOrUpdatedMembers
            });
        }



    }
}
