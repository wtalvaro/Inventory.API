namespace Inventory.API.Models;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty; // CNPJ ou similar
    public string ContactEmail { get; set; } = string.Empty;
    public string? Phone { get; set; }

    // Relacionamento com produtos que ele fornece
    public List<Product> Products { get; set; } = new();

    // ADICIONE ESTA LINHA: Relacionamento para o histórico de compras
    public List<PurchaseOrder> PurchaseOrders { get; set; } = new();
}