using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Umbrella_Server.Data.Repositories.Users;
using Umbrella_Server.DTOs.User;
using Umbrella_Server.Models;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // ✅ GET all users (For Testing - Remove bypass after testing)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
    {
        // 🚀 TEMPORARY: Bypass JWT for testing (Uncomment for authentication)
        // var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // if (userIdClaim == null) return Unauthorized();
        // var userId = Guid.Parse(userIdClaim);

        var users = await _userRepository.GetAllAsync();

        if (!users.Any())
        {
            return NotFound(new { Message = "No users found." });
        }

        var userDtos = users.Select(user => new UserResponseDto
        {
            UserID = user.UserID,
            Name = user.Name,
            Email = user.Email,
            DateCreated = user.DateCreated,
            Roles = user.Roles.Select(r => r.ToString()).ToList(),
            Latitude = user.Latitude,
            Longitude = user.Longitude
        }).ToList();

        return Ok(userDtos);
    }


    // ✅ POST create a new user
    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] UserCreateDto userCreateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // 🚀 Extract User ID from Azure Authentication JWT (Commented out for now)
        // var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // if (userIdClaim == null) return Unauthorized();
        // var userId = Guid.Parse(userIdClaim); // ✅ Use Azure's provided UserID

        // 🚀 For Development: Use a Hardcoded User ID Until Azure Auth is Integrated
        var userId = Guid.NewGuid(); // Replace this when using Azure Auth

        var existingUser = await _userRepository.GetUserByEmailAsync(userCreateDto.Email);
        if (existingUser != null)
            return Conflict(new { Message = "User with this email already exists." });

        var newUser = new User
        {
            UserID = userId, // ✅ Use Azure Auth's ID instead of letting the DB auto-generate it
            Name = userCreateDto.Name,
            Email = userCreateDto.Email,
            Roles = new List<UserRole> { UserRole.Attendee }
        };

        await _userRepository.AddAsync(newUser);
        await _userRepository.SaveChangesAsync();
        var responseDto = new UserResponseDto
        {
            // REMOVE UserID from response after testing
            UserID = newUser.UserID,  // ❌ REMOVE after testing

            Name = newUser.Name,
            Email = newUser.Email,
            DateCreated = newUser.DateCreated,
            Roles = newUser.Roles.Select(r => r.ToString()).ToList()
        };

        // REMOVE UserID from CreatedAtAction after testing
        return CreatedAtAction(nameof(GetCurrentUser), responseDto); // ❌ REMOVE id after testing
    }


    // ✅ GET user 
    [HttpGet("me")]
    public async Task<ActionResult<UserResponseDto>> GetCurrentUser()
    {
        // Replace this with actual JWT claims extraction when Azure Auth is implemented
        // var userEmailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
        // if (userEmailClaim == null) return Unauthorized();

        // For development: Hardcode an email or accept it as a query param
        var userEmail = "test@example.com"; // Replace with actual test data

        var user = await _userRepository.GetUserByEmailAsync(userEmail);
        if (user == null) return NotFound();

        var responseDto = new UserResponseDto
        {
            UserID = user.UserID,
            Name = user.Name,
            Email = user.Email,
            DateCreated = user.DateCreated,
            Roles = user.Roles.Select(r => r.ToString()).ToList(),
            Latitude = user.Latitude,
            Longitude = user.Longitude
        };

        return Ok(responseDto);
    }


    // ✅ DELETE user by ID
    //TBD later

    //ROLES
    // ✅ GET current user roles
    [HttpGet("me/roles")]
    public async Task<ActionResult<List<string>>> GetCurrentUserRoles()
    {
        // Extract User ID from JWT (Commented out until Azure Auth is enabled)
        // var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // if (userIdClaim == null) return Unauthorized();

        // For Development: Use a hardcoded User ID instead
        var user = await _userRepository.GetByIdAsync(Guid.Parse("YOUR_TEST_USER_ID"));

        if (user == null) return NotFound();

        return Ok(user.Roles.Select(r => r.ToString()).ToList());
    }


    // ✅ PUT update user roles
    [HttpPut("me/roles")]
    public async Task<IActionResult> UpdateUserRoles([FromBody] UserRoleUpdateDto roleUpdateDto)
    {
        // Extract User ID from JWT (Commented out until Azure Auth is enabled)
        // var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // if (userIdClaim == null) return Unauthorized();

        // For Development: Use a hardcoded User ID instead
        var userToUpdate = await _userRepository.GetByIdAsync(Guid.Parse("YOUR_TEST_USER_ID"));

        if (userToUpdate == null) return NotFound();

        // Check if the user has Admin privileges (Commented out until JWT roles are used)
        // var userRolesClaim = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
        // if (!userRolesClaim.Contains("Admin")) return Forbid("Only admins can update roles.");

        // Apply new roles (converted from string)
        userToUpdate.Roles = roleUpdateDto.Roles.Select(r => Enum.Parse<UserRole>(r)).ToList();

        _userRepository.Update(userToUpdate);
        await _userRepository.SaveChangesAsync();

        return Ok(new { Message = "Your roles have been updated successfully." });
    }


}
