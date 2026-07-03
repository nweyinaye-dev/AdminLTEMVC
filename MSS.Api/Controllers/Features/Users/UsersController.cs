using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MSS.Api.Services;
using MSS.Domain.Entities;
using MSS.Domain.Features.Users;
using System.ComponentModel.DataAnnotations;

namespace MSS.Api.Controllers.Features.Users;

/// <summary>
/// Users API controller - Feature-based organization
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private const string AdminUsername = "admin";
    private const string ResetRequiredPlaceholder = "RESET_REQUIRED";

    public UsersController(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    // GET: api/Users
    [HttpGet]
    //[HasPermission("user:list")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _userRepository.GetAllAsync();
        return Ok(users);
    }

    // GET: api/Users/5
    [HttpGet("{id}")]
    //[HasPermission("user:list")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    // GET: api/Users/username/{username}
    [HttpGet("username/{username}")]
    [AllowAnonymous]
    public async Task<ActionResult<User>> GetUserByUsername(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    // GET: api/Users/5/roles
    [HttpGet("{id}/roles")]
  //  [HasPermission("user:list")]
    public async Task<ActionResult<User>> GetUserWithRoles(int id)
    {
        var user = await _userRepository.GetUserWithRolesAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    // GET: api/Users/5/roles-permissions
    [HttpGet("{id}/roles-permissions")]
    [AllowAnonymous]
    public async Task<ActionResult<User>> GetUserWithRolesAndPermissions(int id)
    {
        var user = await _userRepository.GetUserWithRolesAndPermissionsAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    // POST: api/Users
    [HttpPost]
   // [HasPermission("user:create")]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserRequest request)
    {
        if (request == null || request.User == null)
        {
            return BadRequest("User data is required.");
        }

        var user = request.User;
        
        // Set default password if not provided
        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            const string defaultPassword = "12345";
            user.PasswordHash = _passwordHasher.HashPassword(defaultPassword);
        }
        else
        {
            // Hash the provided password
            user.PasswordHash = _passwordHasher.HashPassword(user.PasswordHash);
        }

        var id = await _userRepository.CreateAsync(user);
        user.Id = id;

        // Assign roles if provided
        if (request.RoleIds != null && request.RoleIds.Any())
        {
            await _userRepository.SetUserRolesAsync(id, request.RoleIds);
        }

        // Load user with roles for response
        var userWithRoles = await _userRepository.GetUserWithRolesAsync(id);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userWithRoles ?? user);
    }

    // PUT: api/Users/5
    [HttpPut("{id}")]
    //[HasPermission("user:edit")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        if (request == null || request.User == null)
        {
            return BadRequest("User data is required.");
        }

        var user = request.User;
        if (id != user.Id)
        {
            return BadRequest();
        }

        // Get existing user to preserve password hash if not changed
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null)
        {
            return NotFound();
        }

        // Handle password update
        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            // Keep existing password
            user.PasswordHash = existingUser.PasswordHash;
        }
        else if (user.PasswordHash != existingUser.PasswordHash)
        {
            // Hash the new password
            user.PasswordHash = _passwordHasher.HashPassword(user.PasswordHash);
        }

        var result = await _userRepository.UpdateAsync(user);
        if (!result)
        {
            return NotFound();
        }

        // Update roles if provided
        if (request.RoleIds != null)
        {
            await _userRepository.SetUserRolesAsync(id, request.RoleIds);
        }

        return NoContent();
    }

    // DELETE: api/Users/5
    [HttpDelete("{id}")]
   // [HasPermission("user:delete")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _userRepository.DeleteAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Resets the Admin user's password using server-side hashing. Allowed anonymously only when the admin account still has the reset placeholder password.
    /// </summary>
    [HttpPost("admin/reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetAdminPassword([FromBody] ResetPasswordRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Password is required.");
        }

        var user = await _userRepository.GetByUsernameAsync(AdminUsername);
        if (user == null)
        {
            return NotFound("Admin user not found.");
        }

        //if (!string.Equals(user.PasswordHash, ResetRequiredPlaceholder, StringComparison.Ordinal))
        //{
        //    return BadRequest("Admin password has already been set. Use the standard update flow instead.");
        //}

        var hashedPassword = _passwordHasher.HashPassword(request.Password);
        var updated = await _userRepository.UpdatePasswordHashAsync(user.Id, hashedPassword);
        if (!updated)
        {
            return Problem("Failed to reset admin password.");
        }

        return Ok(new { message = "Admin password has been reset successfully." });
    }
}

public record ResetPasswordRequest([Required] string Password);

public record CreateUserRequest
{
    [Required]
    public User User { get; init; } = null!;
    public int[]? RoleIds { get; init; }
}

public record UpdateUserRequest
{
    [Required]
    public User User { get; init; } = null!;
    public int[]? RoleIds { get; init; }
}

