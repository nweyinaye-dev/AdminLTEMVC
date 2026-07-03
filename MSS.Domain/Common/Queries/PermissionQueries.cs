namespace MSS.Domain.Common.Queries;
/// <summary>
/// SQL query constants for Permission operations
/// </summary>
public static class PermissionQueries
{
    public const string GetById = @"
        SELECT Id, Name, Description
        FROM Permissions
        WHERE Id = @Id";

    public const string GetByName = @"
        SELECT Id, Name, Description
        FROM Permissions
        WHERE Name = @Name";

    public const string GetAll = @"
        SELECT Id, Name, Description
        FROM Permissions
        ORDER BY Name";

    public const string GetByRoleId = @"
        SELECT p.Id, p.Name, p.Description
        FROM Permissions p
        INNER JOIN RolePermissions rp ON p.Id = rp.PermissionId
        WHERE rp.RoleId = @RoleId
        ORDER BY p.Name";

    public const string GetByUserId = @"
        SELECT DISTINCT p.Id, p.Name, p.Description
        FROM Permissions p
        INNER JOIN RolePermissions rp ON p.Id = rp.PermissionId
        INNER JOIN UserRoles ur ON rp.RoleId = ur.RoleId
        WHERE ur.UserId = @UserId
        ORDER BY p.Name";

    public const string Insert = @"
        INSERT INTO Permissions (Name, Description)
        OUTPUT INSERTED.Id
        VALUES (@Name, @Description)";

    public const string Update = @"
        UPDATE Permissions
        SET Name = @Name,
            Description = @Description
        WHERE Id = @Id";

    public const string Delete = @"
        DELETE FROM Permissions
        WHERE Id = @Id";
}

