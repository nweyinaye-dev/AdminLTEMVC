namespace MSS.Domain.Common.Queries;

/// <summary>
/// SQL query constants for User operations
/// </summary>
public static class UserQueries
{
    public const string GetById = @"
        SELECT Id, Username, Email, PasswordHash, IsActive, CreatedAt, UpdatedAt
        FROM Users
        WHERE Id = @Id";

    public const string GetByUsername = @"
        SELECT Id, Username, Email, PasswordHash, IsActive, CreatedAt, UpdatedAt
        FROM Users
        WHERE Username = @Username";

    public const string GetByEmail = @"
        SELECT Id, Username, Email, PasswordHash, IsActive, CreatedAt, UpdatedAt
        FROM Users
        WHERE Email = @Email";

    public const string GetAll = @"
        SELECT Id, Username, Email, PasswordHash, IsActive, CreatedAt, UpdatedAt
        FROM Users
        ORDER BY CreatedAt DESC";

    public const string Insert = @"
        INSERT INTO Users (Username, Email, PasswordHash, IsActive, CreatedAt, UpdatedAt)
        OUTPUT INSERTED.Id
        VALUES (@Username, @Email, @PasswordHash, @IsActive, @CreatedAt, @UpdatedAt)";

    public const string Update = @"
        UPDATE Users
        SET Username = @Username,
            Email = @Email,
            PasswordHash = @PasswordHash,
            IsActive = @IsActive,
            UpdatedAt = @UpdatedAt
        WHERE Id = @Id";

    public const string UpdatePasswordHash = @"
        UPDATE Users
        SET PasswordHash = @PasswordHash,
            UpdatedAt = @UpdatedAt
        WHERE Id = @Id";

    public const string Delete = @"
        DELETE FROM Users
        WHERE Id = @Id";

    public const string GetUserWithRoles = @"
        SELECT u.Id, u.Username, u.Email, u.PasswordHash, u.IsActive, u.CreatedAt, u.UpdatedAt,
               r.Id, r.Name, r.Description, r.IsActive, r.CreatedAt, r.UpdatedAt
        FROM Users u
        LEFT JOIN UserRoles ur ON u.Id = ur.UserId
        LEFT JOIN Roles r ON ur.RoleId = r.Id
        WHERE u.Id = @UserId";

    public const string GetUserWithRolesAndPermissions = @"
        SELECT u.Id, u.Username, u.Email, u.PasswordHash, u.IsActive, u.CreatedAt, u.UpdatedAt,
               r.Id, r.Name, r.Description, r.IsActive, r.CreatedAt, r.UpdatedAt,
               p.Id, p.Name, p.Description
        FROM Users u
        LEFT JOIN UserRoles ur ON u.Id = ur.UserId
        LEFT JOIN Roles r ON ur.RoleId = r.Id
        LEFT JOIN RolePermissions rp ON r.Id = rp.RoleId
        LEFT JOIN Permissions p ON rp.PermissionId = p.Id
        WHERE u.Id = @UserId";
}

