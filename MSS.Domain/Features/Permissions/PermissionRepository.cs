
using MSS.Domain.Common;
using MSS.Domain.Common.Queries;
using MSS.Domain.Entities;

namespace MSS.Domain.Features.Permissions;

/// <summary>
/// Permission repository implementation
/// </summary>
public class PermissionRepository : IPermissionRepository
{
    private readonly IDapperService _dapperService;

    public PermissionRepository(IDapperService dapperService)
    {
        _dapperService = dapperService;
    }

    public async Task<Permission?> GetByIdAsync(int id)
    {
        return await _dapperService.QueryFirstOrDefaultAsync<Permission>(
            PermissionQueries.GetById,
            new { Id = id }
        );
    }

    public async Task<Permission?> GetByNameAsync(string name)
    {
        return await _dapperService.QueryFirstOrDefaultAsync<Permission>(
            PermissionQueries.GetByName,
            new { Name = name }
        );
    }

    public async Task<IEnumerable<Permission>> GetAllAsync()
    {
        return await _dapperService.QueryAsync<Permission>(PermissionQueries.GetAll);
    }

    public async Task<IEnumerable<Permission>> GetByRoleIdAsync(int roleId)
    {
        return await _dapperService.QueryAsync<Permission>(
            PermissionQueries.GetByRoleId,
            new { RoleId = roleId }
        );
    }

    public async Task<IEnumerable<Permission>> GetByUserIdAsync(int userId)
    {
        return await _dapperService.QueryAsync<Permission>(
            PermissionQueries.GetByUserId,
            new { UserId = userId }
        );
    }

    public async Task<int> CreateAsync(Permission permission)
    {
        var id = await _dapperService.ExecuteScalarAsync<int>(
            PermissionQueries.Insert,
            permission
        );

        return id;
    }

    public async Task<bool> UpdateAsync(Permission permission)
    {
        var rowsAffected = await _dapperService.ExecuteAsync(
            PermissionQueries.Update,
            permission
        );

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var rowsAffected = await _dapperService.ExecuteAsync(
            PermissionQueries.Delete,
            new { Id = id }
        );

        return rowsAffected > 0;
    }
}

