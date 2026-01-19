namespace Inventory.API.Models;

public class PurchaseOrderItem
{
    public int Id { get; set; }

    // Chave Estrangeira
    public int PurchaseOrderId { get; set; }

    // PROPRIEDADE DE NAVEGAÇÃO (Faltava esta linha para o Service funcionar)
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int QuantityOrdered { get; set; }
    public decimal UnitCost { get; set; }
}