
using MSS.Domain.Common;
using MSS.Domain.Common.Queries;
using MSS.Domain.Entities;

namespace MSS.Domain.Features.Products;

/// <summary>
/// Product repository implementation
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly IDapperService _dapperService;

    public ProductRepository(IDapperService dapperService)
    {
        _dapperService = dapperService;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _dapperService.QueryFirstOrDefaultAsync<Product>(
            ProductQueries.GetById,
            new { Id = id }
        );
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dapperService.QueryAsync<Product>(ProductQueries.GetAll);
    }

    public async Task<IEnumerable<Product>> GetActiveAsync()
    {
        return await _dapperService.QueryAsync<Product>(ProductQueries.GetActive);
    }

    public async Task<IEnumerable<Product>> SearchAsync(string? searchTerm)
    {
        return await _dapperService.QueryAsync<Product>(
            ProductQueries.Search,
            new { SearchTerm = searchTerm }
        );
    }

    public async Task<int> CreateAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;

        var id = await _dapperService.ExecuteScalarAsync<int>(
            ProductQueries.Insert,
            product
        );

        return id;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;

        var rowsAffected = await _dapperService.ExecuteAsync(
            ProductQueries.Update,
            product
        );

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var rowsAffected = await _dapperService.ExecuteAsync(
            ProductQueries.Delete,
            new { Id = id }
        );

        return rowsAffected > 0;
    }
}

