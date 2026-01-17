using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Services;

public class InventoryService : IInventoryService
{
    private readonly InventoryDbContext _context;

    public InventoryService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StoreInventory>> GetCatalogAsync(int storeId)
    {
        var query = _context.StoreInventories
            .Include(si => si.Product)
            .Include(si => si.Store) // Adicionado: Útil para o Coordenador saber de qual loja é o item
            .AsQueryable();

        // Se storeId for > 0, filtramos. Se for 0, trazemos TUDO (Visão Global).
        if (storeId > 0)
        {
            query = query.Where(si => si.StoreId == storeId);
        }

        return await query.ToListAsync();
    }

    public async Task<StoreInventory?> GetBySkuAsync(int storeId, string sku)
    {
        var query = _context.StoreInventories
            .Include(si => si.Product)
            .Include(si => si.Store);

        // Busca global por SKU se storeId for 0
        if (storeId > 0)
        {
            return await query.FirstOrDefaultAsync(si => si.StoreId == storeId && si.SKU == sku);
        }

        return await query.FirstOrDefaultAsync(si => si.SKU == sku);
    }

    public async Task<StoreInventory?> UpdateItemAsync(int id, StoreInventory updateData)
    {
        var item = await _context.StoreInventories.FindAsync(id);
        if (item == null) return null;

        item.Quantity = updateData.Quantity;
        item.LocalPrice = updateData.LocalPrice;
        item.Aisle = updateData.Aisle;
        item.Shelf = updateData.Shelf;
        item.VariantData = updateData.VariantData;

        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<IEnumerable<StoreInventory>> GetLowStockAsync(int storeId, int threshold)
    {
        var query = _context.StoreInventories
            .Include(si => si.Product)
            .Include(si => si.Store)
            .Where(si => si.Quantity <= threshold)
            .AsQueryable();

        if (storeId > 0)
        {
            query = query.Where(si => si.StoreId == storeId);
        }

        return await query.OrderBy(si => si.Quantity).ToListAsync();
    }

    public async Task<StoreInventory> AddToInventoryAsync(StoreInventory newItem)
    {
        _context.StoreInventories.Add(newItem);
        await _context.SaveChangesAsync();
        return newItem;
    }
}