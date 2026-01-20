using Inventory.API.Models;

public interface IPurchaseOrderService
{
    Task<IEnumerable<PurchaseOrder>> GetAllOrdersAsync(); // Adicione esta linha
    Task<PurchaseOrder> CreateOrderAsync(PurchaseOrder order);
    Task<PurchaseOrder?> GetOrderByIdAsync(int id);
    Task<bool> ReceiveOrderAsync(int orderId);
    Task<IEnumerable<PurchaseOrder>> GetOrdersByStoreAsync(int storeId);
}