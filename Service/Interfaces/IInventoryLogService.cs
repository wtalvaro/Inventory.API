using Inventory.API.Models;

namespace Inventory.API.Services.Interfaces;

public interface IInventoryLogService
{
    Task<IEnumerable<InventoryLog>> GetLogsAsync(int? storeId = null, int limit = 100);
    Task<IEnumerable<InventoryLog>> GetLogsByProductAsync(int productId);
    Task<IEnumerable<InventoryLog>> GetRecentLogsAsync(int days, int? storeId = null);
}