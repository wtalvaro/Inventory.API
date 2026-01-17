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

    public async Task<IEnumerable<InventoryLog>> GetAllLogsAsync(int limit = 100)
    {
        return await _context.InventoryLogs
            .OrderByDescending(l => l.Timestamp)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<InventoryLog>> GetLogsBySkuAsync(string sku)
    {
        return await _context.InventoryLogs
            .Where(l => l.SKU == sku)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<InventoryLog>> GetRecentLogsAsync(int days)
    {
        var dataLimite = DateTime.UtcNow.AddDays(-days);

        return await _context.InventoryLogs
            .Where(l => l.Timestamp >= dataLimite)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();
    }
}