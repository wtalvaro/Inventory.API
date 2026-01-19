using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Services;

public class InventoryLogService : IInventoryLogService
{
    private readonly InventoryDbContext _context;

    public InventoryLogService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InventoryLog>> GetLogsAsync(int? storeId = null, int limit = 100)
    {
        var query = _context.InventoryLogs
            .Include(l => l.Product)
            .Include(l => l.Store)
            .AsNoTracking();

        if (storeId.HasValue)
            query = query.Where(l => l.StoreId == storeId.Value);

        return await query
            .OrderByDescending(l => l.CreatedAt) // Corrigido: era Timestamp
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<InventoryLog>> GetLogsByProductAsync(int productId)
    {
        return await _context.InventoryLogs
            .Where(l => l.ProductId == productId)
            .OrderByDescending(l => l.CreatedAt) // Corrigido
            .ToListAsync();
    }

    public async Task<IEnumerable<InventoryLog>> GetRecentLogsAsync(int days, int? storeId = null)
    {
        var dataLimite = DateTime.UtcNow.AddDays(-days);
        var query = _context.InventoryLogs.Where(l => l.CreatedAt >= dataLimite); // Corrigido

        if (storeId.HasValue)
            query = query.Where(l => l.StoreId == storeId.Value);

        return await query
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }
}