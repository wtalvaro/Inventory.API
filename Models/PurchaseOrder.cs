using Inventory.API.Models.Enums;

namespace Inventory.API.Models;

public class PurchaseOrder
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public int StoreId { get; set; } // Destino da compra
    public Store Store { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReceivedAt { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public decimal TotalCost { get; set; }
    public List<PurchaseOrderItem> Items { get; set; } = new();
}