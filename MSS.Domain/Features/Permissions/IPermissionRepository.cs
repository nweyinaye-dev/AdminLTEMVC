

using MSS.Domain.Entities;

namespace MSS.Domain.Features.Permissions;

/// <summary>
/// Permission repository interface
/// </summary>
public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(int id);
    Task<Permission?> GetByNameAsync(string name);
    Task<IEnumerable<Permission>> GetAllAsync();
    Task<IEnumerable<Permission>> GetByRoleIdAsync(int roleId);
    Task<IEnumerable<Permission>> GetByUserIdAsync(int userId);
    Task<int> CreateAsync(Permission permission);
    Task<bool> UpdateAsync(Permission permission);
    Task<bool> DeleteAsync(int id);
}

