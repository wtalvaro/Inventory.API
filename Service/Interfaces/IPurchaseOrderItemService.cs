using Inventory.API.Models;

namespace Inventory.API.Services.Interfaces;

public interface IPurchaseOrderItemService
{
    Task<PurchaseOrderItem> AddItemToOrderAsync(PurchaseOrderItem item);
    Task<bool> UpdateItemAsync(int id, int quantity, decimal unitCost);
    Task<bool> RemoveItemAsync(int id);
    Task<IEnumerable<PurchaseOrderItem>> GetItemsByOrderIdAsync(int orderId);
}