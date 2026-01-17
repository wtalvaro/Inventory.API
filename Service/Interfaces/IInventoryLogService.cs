using Inventory.API.Models;

namespace Inventory.API.Services.Interfaces;

public interface IInventoryLogService
{
    Task<IEnumerable<InventoryLog>> GetAllLogsAsync(int limit = 100);
    Task<IEnumerable<InventoryLog>> GetLogsBySkuAsync(string sku);
    Task<IEnumerable<InventoryLog>> GetRecentLogsAsync(int days);
}