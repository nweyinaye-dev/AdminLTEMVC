


using MSS.Domain.Entities;

namespace MSS.Domain.Features.Products;

/// <summary>
/// Product repository interface
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetActiveAsync();
    Task<IEnumerable<Product>> SearchAsync(string? searchTerm);
    Task<int> CreateAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
}

