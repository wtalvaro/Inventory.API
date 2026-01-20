namespace Inventory.API.Models;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty; // CNPJ ou similar
    public string ContactEmail { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public List<Product> Products { get; set; } = new();
    public List<PurchaseOrder> PurchaseOrders { get; set; } = new();
    public decimal BaseOrderFee { get; set; }
    public decimal DefaultShippingFee { get; set; }
}