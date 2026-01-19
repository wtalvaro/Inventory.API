using Inventory.API.Models;

namespace Inventory.API.Services.Interfaces;

public interface IStockTransferItemService
{
    Task<IEnumerable<StockTransferItem>> GetByTransferIdAsync(int transferId);
    Task<StockTransferItem?> GetByIdAsync(int id);
    Task<StockTransferItem> AddAsync(StockTransferItem item);
    Task<bool> UpdateAsync(int id, StockTransferItem item);
    Task<bool> DeleteAsync(int id);
}