using Microsoft.AspNetCore.Mvc;
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
                UserID = newUser.UserID,
                Name = newUser.Name,
                Email = newUser.Email,
                DateCreated = newUser.DateCreated,
                Roles = newUser.Roles.Select(r => r.ToString()).ToList()
            };

            return CreatedAtAction(nameof(GetUser), new { id = newUser.UserID }, responseDto);
        }

    // ✅ GET user by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetUser(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
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
}
