using System.ComponentModel.DataAnnotations;

namespace Inventory.API.Models;

public class CartItem
{
    [Key]
    public int Id { get; set; }
    public int SalesSessionId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}