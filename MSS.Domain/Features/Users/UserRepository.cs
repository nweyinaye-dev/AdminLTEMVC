
using Dapper;
using MSS.Domain.Common;
using MSS.Domain.Common.Queries;
using MSS.Domain.Entities;

namespace MSS.Domain.Features.Users;

/// <summary>
/// User repository implementation
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly IDapperService _dapperService;

    public UserRepository(IDapperService dapperService)
    {
        _dapperService = dapperService;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _dapperService.QueryFirstOrDefaultAsync<User>(
            UserQueries.GetById,
            new { Id = id }
        );
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dapperService.QueryFirstOrDefaultAsync<User>(
            UserQueries.GetByUsername,
            new { Username = username }
        );
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dapperService.QueryFirstOrDefaultAsync<User>(
            UserQueries.GetByEmail,
            new { Email = email }
        );
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dapperService.QueryAsync<User>(UserQueries.GetAll);
    }

    public async Task<User?> GetUserWithRolesAsync(int userId)
    {
        var userDict = new Dictionary<int, User>();

        var results = await _dapperService.QueryAsync<User, Role, User>(
            UserQueries.GetUserWithRoles,
            (user, role) =>
            {
                if (!userDict.TryGetValue(user.Id, out var userEntry))
                {
                    userEntry = user;
                    userEntry.Roles = new List<Role>();
                    userDict.Add(user.Id, userEntry);
                }

                if (role != null && !userEntry.Roles.Any(r => r.Id == role.Id))
                {
                    userEntry.Roles.Add(role);
                }

                return userEntry;
            },
            new { UserId = userId },
            splitOn: "Id"
        );

        return userDict.Values.FirstOrDefault();
    }

    public async Task<User?> GetUserWithRolesAndPermissionsAsync(int userId)
    {
        var userDict = new Dictionary<int, User>();
        var roleDict = new Dictionary<int, Role>();

        var results = await _dapperService.QueryAsync<User, Role, Permission, User>(
            UserQueries.GetUserWithRolesAndPermissions,
            (user, role, permission) =>
            {
                if (!userDict.TryGetValue(user.Id, out var userEntry))
                {
                    userEntry = user;
                    userEntry.Roles = new List<Role>();
                    userEntry.Permissions = new List<Permission>();
                    userDict.Add(user.Id, userEntry);
                }

                if (role != null)
                {
                    if (!roleDict.TryGetValue(role.Id, out var roleEntry))
                    {
                        roleEntry = role;
                        roleEntry.Permissions = new List<Permission>();
                        roleDict.Add(role.Id, roleEntry);
                        userEntry.Roles.Add(roleEntry);
                    }

                    if (permission != null && !roleEntry.Permissions.Any(p => p.Id == permission.Id))
                    {
                        roleEntry.Permissions.Add(permission);
                    }

                    if (permission != null && !userEntry.Permissions.Any(p => p.Id == permission.Id))
                    {
                        userEntry.Permissions.Add(permission);
                    }
                }

                return userEntry;
            },
            new { UserId = userId },
            splitOn: "Id,Id"
        );

        return userDict.Values.FirstOrDefault();
    }

    public async Task<int> CreateAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        var id = await _dapperService.ExecuteScalarAsync<int>(
            UserQueries.Insert,
            user
        );

        return id;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;

        var rowsAffected = await _dapperService.ExecuteAsync(
            UserQueries.Update,
            user
        );

        return rowsAffected > 0;
    }

    public async Task<bool> UpdatePasswordHashAsync(int id, string passwordHash)
    {
        var rowsAffected = await _dapperService.ExecuteAsync(
            UserQueries.UpdatePasswordHash,
            new
            {
                Id = id,
                PasswordHash = passwordHash,
                UpdatedAt = DateTime.UtcNow
            }
        );

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var rowsAffected = await _dapperService.ExecuteAsync(
            UserQueries.Delete,
            new { Id = id }
        );

        return rowsAffected > 0;
    }

    public async Task<bool> AssignRoleAsync(int userId, int roleId)
    {
        const string sql = @"
            IF NOT EXISTS (SELECT 1 FROM UserRoles WHERE UserId = @UserId AND RoleId = @RoleId)
            BEGIN
                INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)
            END";

        var rowsAffected = await _dapperService.ExecuteAsync(
            sql,
            new { UserId = userId, RoleId = roleId }
        );

        return rowsAffected >= 0; // Can be 0 if role already assigned
    }

    public async Task<bool> RemoveRoleAsync(int userId, int roleId)
    {
        const string sql = @"
            DELETE FROM UserRoles
            WHERE UserId = @UserId AND RoleId = @RoleId";

        var rowsAffected = await _dapperService.ExecuteAsync(
            sql,
            new { UserId = userId, RoleId = roleId }
        );

        return rowsAffected > 0;
    }

    public async Task<bool> SetUserRolesAsync(int userId, IEnumerable<int> roleIds)
    {
        const string deleteSql = @"
            DELETE FROM UserRoles
            WHERE UserId = @UserId";

        const string insertSql = @"
            INSERT INTO UserRoles (UserId, RoleId)
            VALUES (@UserId, @RoleId)";

        // Remove all existing roles
        await _dapperService.ExecuteAsync(deleteSql, new { UserId = userId });

        // Add new roles
        var roleIdList = roleIds.ToList();
        if (roleIdList.Any())
        {
            foreach (var roleId in roleIdList)
            {
                await _dapperService.ExecuteAsync(insertSql, new { UserId = userId, RoleId = roleId });
            }
        }

        return true;
    }
}

