namespace Inventory.API.Models;

public class StockBatch
{
    public int Id { get; set; }

    // Foreign Keys
    public int ProductId { get; set; }
    public int StoreId { get; set; }
    public int? PurchaseOrderId { get; set; }

    // ADICIONE ESTAS PROPRIEDADES DE NAVEGAÇÃO:
    public Product Product { get; set; } = null!;
    public Store Store { get; set; } = null!;
    public PurchaseOrder? PurchaseOrder { get; set; }

    public int InitialQuantity { get; set; }
    public int CurrentQuantity { get; set; }

    public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }

    public decimal UnitCost { get; set; }
}