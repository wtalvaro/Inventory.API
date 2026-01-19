using Inventory.API.Models;
using Inventory.API.Models.Enums;

namespace Inventory.API.Services.Interfaces;

public interface IPurchaseOrderService
{
    Task<PurchaseOrder> CreateOrderAsync(PurchaseOrder order);
    Task<PurchaseOrder?> GetOrderByIdAsync(int id);
    Task<bool> ReceiveOrderAsync(int orderId); // O método "mágico" que gera o estoque
    Task<IEnumerable<PurchaseOrder>> GetOrdersByStoreAsync(int storeId);
}