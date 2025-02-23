using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbrella_Server.Data;
using Umbrella_Server.DTOs.ActiveGroup;
using Umbrella_Server.Models;
using Umbrella_Server.Security;

namespace Umbrella_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActiveGroupController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActiveGroupController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ 1️⃣ Start Tracking When Event Begins
        [HttpPost("{groupId}/start")]
        public async Task<ActionResult> StartTracking(Guid groupId)
        {

            //add logic to check if member
            // var userId = User.GetUserId();

            var group = await _context.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.GroupID == groupId);
            if (group == null) return NotFound(new { Message = "Group not found." });

            if (DateTime.UtcNow < group.StartTime)
                return BadRequest(new { Message = "Event has not started yet." });

            // Initialize ActiveGroup for all members
            var activeGroupMembers = group.Members.Select(m => new ActiveGroup
            {
                GroupID = groupId,
                UserID = m.UserID,
                Latitude = 0, // Default until first update
                Longitude = 0,
                TimeStamp = DateTime.UtcNow
            }).ToList();

            _context.ActiveGroups.AddRange(activeGroupMembers);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Tracking started." });
        }

        // ✅ 2️⃣ Update User Location (Client Sends Every 5 Min)
        [HttpPut("{groupId}/update-location")]
        public async Task<ActionResult> UpdateLocation(Guid groupId, [FromBody] LocationUpdateDto locationDto)
        {

            //add logic to check if member
            // var userId = User.GetUserId();

            var activeMember = await _context.ActiveGroups.FirstOrDefaultAsync(ag => ag.GroupID == groupId && ag.UserID == locationDto.UserID);
            if (activeMember == null)
                return NotFound(new { Message = "User is not active in this group." });

            activeMember.Latitude = locationDto.Latitude;
            activeMember.Longitude = locationDto.Longitude;
            activeMember.TimeStamp = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Location updated." });
        }

        // ✅ 3️⃣ Get All Members' Locations (Organizer Requests Locations)
        [HttpGet("{groupId}/locations")]
        public async Task<ActionResult<IEnumerable<ActiveGroup>>> GetLocations(Guid groupId)
        {

            //add logic to check if member
            // var userId = User.GetUserId();
            var locations = await _context.ActiveGroups.Where(ag => ag.GroupID == groupId).ToListAsync();
            return Ok(locations);
        }

        // ✅ 4️⃣ End Tracking (Cleanup After Event Ends)
        [HttpDelete("{groupId}/end")]
        public async Task<ActionResult> EndTracking(Guid groupId)
        {

            //add logic to check if organizer
            // var userId = User.GetUserId();
            var activeMembers = _context.ActiveGroups.Where(ag => ag.GroupID == groupId);
            if (!activeMembers.Any()) return NotFound(new { Message = "No active tracking found for this group." });

            _context.ActiveGroups.RemoveRange(activeMembers);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Tracking ended and data cleared." });
        }
    }
}
