using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Services;

public class StoreService : IStoreService
{
    private readonly InventoryDbContext _context;

    public StoreService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Store>> GetAllActiveAsync()
    {
        return await _context.Stores
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Store?> GetByIdAsync(int id)
    {
        return await _context.Stores.FindAsync(id);
    }

    public async Task<Store> CreateAsync(Store store)
    {
        store.IsActive = true; // Por padrão, uma nova loja nasce ativa
        _context.Stores.Add(store);
        await _context.SaveChangesAsync();
        return store;
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store == null) return false;

        store.IsActive = false; // Soft Delete (Desativação em vez de exclusão física)
        await _context.SaveChangesAsync();
        return true;
    }
}