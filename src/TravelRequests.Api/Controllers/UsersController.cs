using Microsoft.AspNetCore.Mvc;
using TravelRequests.Application.Contracts.Repositories;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;

namespace TravelRequests.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _userRepository.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpGet("by-email/{email}")]
    public async Task<ActionResult<User>> GetUserByEmail(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpGet("by-role/{role}")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole(UserRole role)
    {
        var users = await _userRepository.GetByRoleAsync(role);
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserRequest request)
    {
        // Verificar si el email ya existe
        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            return BadRequest("Email already exists");
        }

        // Crear nuevo usuario
        var user = new User(request.Name, request.Email, request.PasswordHash, request.Role);
        var createdUser = await _userRepository.AddAsync(user);

        return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
    }

    [HttpGet("with-travel-requests")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsersWithTravelRequests([FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        var users = await _userRepository.GetUsersWithTravelRequestsAsync(skip, take);
        return Ok(users);
    }
}

public record CreateUserRequest(
    string Name,
    string Email,
    string PasswordHash,
    UserRole Role
);
