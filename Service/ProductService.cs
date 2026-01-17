using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Services;

public class ProductService : IProductService
{
    private readonly InventoryDbContext _context;

    public ProductService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync(string? category, string? subCategory, string? gender, string? targetAudience)
    {
        var query = _context.Products.AsQueryable();

        // Lógica de filtro (O CRUD de Leitura vive aqui)
        if (!string.IsNullOrEmpty(category)) query = query.Where(p => p.Category == category);
        if (!string.IsNullOrEmpty(subCategory)) query = query.Where(p => p.SubCategory == subCategory);
        if (!string.IsNullOrEmpty(gender)) query = query.Where(p => p.Gender == gender);
        if (!string.IsNullOrEmpty(targetAudience)) query = query.Where(p => p.TargetAudience == targetAudience);

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id) => await _context.Products.FindAsync(id);

    public async Task<Product> CreateAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> UpdateAsync(int id, Product product)
    {
        if (id != product.Id) return false;
        _context.Entry(product).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}