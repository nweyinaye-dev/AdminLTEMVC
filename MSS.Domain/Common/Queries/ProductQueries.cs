namespace MSS.Domain.Common.Queries;

/// <summary>
/// SQL query constants for Product operations
/// </summary>
public static class ProductQueries
{
    public const string GetById = @"
        SELECT Id, Name, Description, Price, Stock, IsActive, CreatedAt, UpdatedAt, CreatedBy
        FROM Products
        WHERE Id = @Id";

    public const string GetAll = @"
        SELECT Id, Name, Description, Price, Stock, IsActive, CreatedAt, UpdatedAt, CreatedBy
        FROM Products
        ORDER BY CreatedAt DESC";

    public const string GetActive = @"
        SELECT Id, Name, Description, Price, Stock, IsActive, CreatedAt, UpdatedAt, CreatedBy
        FROM Products
        WHERE IsActive = 1
        ORDER BY CreatedAt DESC";

    public const string Insert = @"
        INSERT INTO Products (Name, Description, Price, Stock, IsActive, CreatedAt, UpdatedAt, CreatedBy)
        OUTPUT INSERTED.Id
        VALUES (@Name, @Description, @Price, @Stock, @IsActive, @CreatedAt, @UpdatedAt, @CreatedBy)";

    public const string Update = @"
        UPDATE Products
        SET Name = @Name,
            Description = @Description,
            Price = @Price,
            Stock = @Stock,
            IsActive = @IsActive,
            UpdatedAt = @UpdatedAt
        WHERE Id = @Id";

    public const string Delete = @"
        DELETE FROM Products
        WHERE Id = @Id";

    public const string Search = @"
        SELECT Id, Name, Description, Price, Stock, IsActive, CreatedAt, UpdatedAt, CreatedBy
        FROM Products
        WHERE (@SearchTerm IS NULL OR Name LIKE '%' + @SearchTerm + '%' OR Description LIKE '%' + @SearchTerm + '%')
        ORDER BY CreatedAt DESC";
}

