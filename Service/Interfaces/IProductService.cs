using Inventory.API.Models;

namespace Inventory.API.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync(string? category, string? subCategory, string? gender, string? targetAudience);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<bool> UpdateAsync(int id, Product product);
    Task<bool> DeleteAsync(int id);
}