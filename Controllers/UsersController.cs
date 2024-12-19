using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbrella_Server.Data;
using Umbrella_Server.Models;

namespace Umbrella_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { Message = $"User with ID {id} not found." });
            }

            return Ok(user);
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Name))
                {
                    return BadRequest(new { Message = "Name and Email are required fields." });
                }

                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    return Conflict(new { Message = "User with this email already exists." });
                }

                user.UserID = Guid.NewGuid();
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { id = user.UserID }, user);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new
                {
                    Message = "Failed to create user.",
                    Error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        // PUT: api/User/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.UserID)
            {
                return BadRequest(new { Message = "User ID in the URL does not match the body." });
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound(new { Message = $"User with ID {id} not found." });
            }

            existingUser.Name = updatedUser.Name;
            existingUser.Email = updatedUser.Email;
            existingUser.Roles = updatedUser.Roles;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/User/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = $"User with ID {id} not found." });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // PUT: api/User/{id}/roles (Change roles to a user)
        [HttpPut("{id}/roles")]
        public async Task<IActionResult> UpdateUserRoles(Guid id, [FromBody] UserRole[] roles)
        {
            if (roles == null || roles.Length == 0)
            {
                return BadRequest(new { Message = "The roles field is required and cannot be empty." });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = $"User with ID {id} not found." });
            }

            user.Roles = roles.Distinct().ToList();

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Roles successfully updated.", CurrentRoles = user.Roles.Select(r => r.ToString()).ToList() });
        }


        // DELETE: api/User/{id}/roles (Remove roles from a user)
        [HttpDelete("{id}/roles")]
        public async Task<IActionResult> RemoveRolesFromUser(Guid id, [FromBody] UserRole[] roles)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = $"User with ID {id} not found." });
            }

            foreach (var role in roles)
            {
                if (user.Roles.Contains(role))
                {
                    user.Roles.Remove(role); 
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Roles successfully removed.", CurrentRoles = user.Roles.Select(r => r.ToString()).ToList() });
        }


        // GET: api/User/{id}/role (Get the role for a user)
        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetUserRoles(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = $"User with ID {id} not found." });
            }

            return Ok(new { Roles = user.Roles.Select(r => r.ToString()).ToList() });
        }




    }
}
