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

    // ✅ GET all users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
    {
        var users = await _userRepository.GetAllAsync();

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

        var existingUser = await _userRepository.GetUserByEmailAsync(userCreateDto.Email);
        if (existingUser != null)
            return Conflict(new { Message = "User with this email already exists." });

        var newUser = new User
        {
            Name = userCreateDto.Name,
            Email = userCreateDto.Email,
            Roles = new List<UserRole> { UserRole.Attendee }
        };

        await _userRepository.AddAsync(newUser);

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


    // ✅ GET user by ID
    [HttpGet("me")]
    public async Task<ActionResult<UserResponseDto>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var user = await _userRepository.GetByIdAsync(Guid.Parse(userIdClaim));
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


    //ROLES
    // ✅ PUT update user roles
    [HttpPut("me/roles")]
    public async Task<IActionResult> UpdateUserRoles([FromBody] UserRoleUpdateDto roleUpdateDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var userToUpdate = await _userRepository.GetByIdAsync(Guid.Parse(userIdClaim));
        if (userToUpdate == null) return NotFound();

        // ✅ Apply new roles (converted from string)
        userToUpdate.Roles = roleUpdateDto.Roles.Select(r => Enum.Parse<UserRole>(r)).ToList();

        _userRepository.Update(userToUpdate);
        await _userRepository.SaveChangesAsync();

        return Ok(new { Message = "Your roles have been updated successfully." });
    }

}
