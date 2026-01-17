using Inventory.API.Models;

namespace Inventory.API.Services.Interfaces;

public interface IInventoryService
{
    Task<IEnumerable<StoreInventory>> GetCatalogAsync(int storeId);
    Task<StoreInventory?> GetBySkuAsync(int storeId, string sku);
    Task<StoreInventory?> UpdateItemAsync(int id, StoreInventory updateData);
    Task<IEnumerable<StoreInventory>> GetLowStockAsync(int storeId, int threshold);
    Task<StoreInventory> AddToInventoryAsync(StoreInventory newItem);
}