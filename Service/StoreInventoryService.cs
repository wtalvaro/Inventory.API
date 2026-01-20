using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Models.Enums;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Services;

public class StoreInventoryService : IStoreInventoryService
{
    private readonly InventoryDbContext _context;

    public StoreInventoryService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StoreInventory>> GetCatalogAsync(int storeId)
    {
        var query = _context.StoreInventories
            .Include(si => si.Product)
            .Include(si => si.Store)
            .AsNoTracking(); // Performance: consulta apenas leitura

        // Se storeId for 0, o Coordenador/Admin está pedindo a visão global
        if (storeId > 0)
        {
            query = query.Where(si => si.StoreId == storeId);
        }

        return await query.ToListAsync();
    }

    public async Task<StoreInventory?> GetBySkuAsync(int storeId, string sku)
    {
        return await _context.StoreInventories
            .Include(si => si.Product)
            .FirstOrDefaultAsync(si => si.StoreId == storeId && si.SKU == sku);
    }

    public async Task<IEnumerable<StoreInventory>> GetItemsBelowMinimumAsync(int storeId)
    {
        var query = _context.StoreInventories
            .Include(si => si.Product)
            .Where(si => si.Quantity <= si.MinimumStock);

        if (storeId > 0)
            query = query.Where(si => si.StoreId == storeId);

        return await query.OrderBy(si => si.Quantity).ToListAsync();
    }

    public async Task<StoreInventory?> AdjustStockAsync(int inventoryId, int newQuantity, string reason)
    {
        var item = await _context.StoreInventories.FindAsync(inventoryId);
        if (item == null) return null;

        // O DELTA representa a variação real do estoque. 
        // Essencial para relatórios de perdas, ganhos e acuradoria.
        int delta = newQuantity - item.Quantity;

        // Registro de Auditoria (Log)
        var log = new InventoryLog
        {
            ProductId = item.ProductId,
            StoreId = item.StoreId,
            QuantityChange = delta,
            // 3. Escolha automática do tipo baseado no sinal da mudança
            Type = delta >= 0 ? MovementType.AdjustmentIn : MovementType.AdjustmentOut,
            Notes = $"Motivo: {reason}",
            CreatedAt = DateTime.UtcNow
        };

        item.Quantity = newQuantity;
        item.LastUpdated = DateTime.UtcNow;

        _context.InventoryLogs.Add(log);
        await _context.SaveChangesAsync();

        return item;
    }
}