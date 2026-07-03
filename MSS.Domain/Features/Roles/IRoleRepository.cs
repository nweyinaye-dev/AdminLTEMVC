

using MSS.Domain.Entities;

namespace MSS.Domain.Features.Roles;

/// <summary>
/// Role repository interface
/// </summary>
public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int id);
    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role?> GetRoleWithPermissionsAsync(int roleId);
    Task<int> CreateAsync(Role role);
    Task<bool> UpdateAsync(Role role);
    Task<bool> DeleteAsync(int id);
    Task<bool> AssignPermissionAsync(int roleId, int permissionId);
    Task<bool> RemovePermissionAsync(int roleId, int permissionId);
}

