
using Dapper;
using MSS.Domain.Common;
using MSS.Domain.Common.Queries;
using MSS.Domain.Entities;
namespace MSS.Domain.Features.Roles;

/// <summary>
/// Role repository implementation
/// </summary>
public class RoleRepository : IRoleRepository
{
    private readonly IDapperService _dapperService;

    public RoleRepository(IDapperService dapperService)
    {
        _dapperService = dapperService;
    }

    public async Task<Role?> GetByIdAsync(int id)
    {
        return await _dapperService.QueryFirstOrDefaultAsync<Role>(
            RoleQueries.GetById,
            new { Id = id }
        );
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _dapperService.QueryFirstOrDefaultAsync<Role>(
            RoleQueries.GetByName,
            new { Name = name }
        );
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _dapperService.QueryAsync<Role>(RoleQueries.GetAll);
    }

    public async Task<Role?> GetRoleWithPermissionsAsync(int roleId)
    {
        var roleDict = new Dictionary<int, Role>();

        var results = await _dapperService.QueryAsync<Role, Permission, Role>(
            RoleQueries.GetRoleWithPermissions,
            (role, permission) =>
            {
                if (!roleDict.TryGetValue(role.Id, out var roleEntry))
                {
                    roleEntry = role;
                    roleEntry.Permissions = new List<Permission>();
                    roleDict.Add(role.Id, roleEntry);
                }

                if (permission != null && !roleEntry.Permissions.Any(p => p.Id == permission.Id))
                {
                    roleEntry.Permissions.Add(permission);
                }

                return roleEntry;
            },
            new { RoleId = roleId },
            splitOn: "Id"
        );

        return roleDict.Values.FirstOrDefault();
    }

    public async Task<int> CreateAsync(Role role)
    {
        role.CreatedAt = DateTime.UtcNow;
        role.UpdatedAt = DateTime.UtcNow;

        var id = await _dapperService.ExecuteScalarAsync<int>(
            RoleQueries.Insert,
            role
        );

        return id;
    }

    public async Task<bool> UpdateAsync(Role role)
    {
        role.UpdatedAt = DateTime.UtcNow;

        var rowsAffected = await _dapperService.ExecuteAsync(
            RoleQueries.Update,
            role
        );

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var rowsAffected = await _dapperService.ExecuteAsync(
            RoleQueries.Delete,
            new { Id = id }
        );

        return rowsAffected > 0;
    }

    public async Task<bool> AssignPermissionAsync(int roleId, int permissionId)
    {
        const string sql = @"
            INSERT INTO RolePermissions (RoleId, PermissionId)
            VALUES (@RoleId, @PermissionId)";

        var rowsAffected = await _dapperService.ExecuteAsync(
            sql,
            new { RoleId = roleId, PermissionId = permissionId }
        );

        return rowsAffected > 0;
    }

    public async Task<bool> RemovePermissionAsync(int roleId, int permissionId)
    {
        const string sql = @"
            DELETE FROM RolePermissions
            WHERE RoleId = @RoleId AND PermissionId = @PermissionId";

        var rowsAffected = await _dapperService.ExecuteAsync(
            sql,
            new { RoleId = roleId, PermissionId = permissionId }
        );

        return rowsAffected > 0;
    }
}

