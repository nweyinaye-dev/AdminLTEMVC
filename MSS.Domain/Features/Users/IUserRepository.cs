
using MSS.Domain.Entities;

namespace MSS.Domain.Features.Users;

/// <summary>
/// User repository interface
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetUserWithRolesAsync(int userId);
    Task<User?> GetUserWithRolesAndPermissionsAsync(int userId);
    Task<int> CreateAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> UpdatePasswordHashAsync(int id, string passwordHash);
    Task<bool> DeleteAsync(int id);
    Task<bool> AssignRoleAsync(int userId, int roleId);
    Task<bool> RemoveRoleAsync(int userId, int roleId);
    Task<bool> SetUserRolesAsync(int userId, IEnumerable<int> roleIds);
}

