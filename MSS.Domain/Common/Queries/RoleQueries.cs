namespace MSS.Domain.Common.Queries;

/// <summary>
/// SQL query constants for Role operations
/// </summary>
public static class RoleQueries
{
    public const string GetById = @"
        SELECT Id, Name, Description, IsActive, CreatedAt, UpdatedAt
        FROM Roles
        WHERE Id = @Id";

    public const string GetByName = @"
        SELECT Id, Name, Description, IsActive, CreatedAt, UpdatedAt
        FROM Roles
        WHERE Name = @Name";

    public const string GetAll = @"
        SELECT Id, Name, Description, IsActive, CreatedAt, UpdatedAt
        FROM Roles
        ORDER BY CreatedAt DESC";

    public const string Insert = @"
        INSERT INTO Roles (Name, Description, IsActive, CreatedAt, UpdatedAt)
        OUTPUT INSERTED.Id
        VALUES (@Name, @Description, @IsActive, @CreatedAt, @UpdatedAt)";

    public const string Update = @"
        UPDATE Roles
        SET Name = @Name,
            Description = @Description,
            IsActive = @IsActive,
            UpdatedAt = @UpdatedAt
        WHERE Id = @Id";

    public const string Delete = @"
        DELETE FROM Roles
        WHERE Id = @Id";

    public const string GetRoleWithPermissions = @"
        SELECT r.Id, r.Name, r.Description, r.IsActive, r.CreatedAt, r.UpdatedAt,
               p.Id, p.Name, p.Description
        FROM Roles r
        LEFT JOIN RolePermissions rp ON r.Id = rp.RoleId
        LEFT JOIN Permissions p ON rp.PermissionId = p.Id
        WHERE r.Id = @RoleId";
}

