using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.API.Models;

public class CartItem
{
    [Key]
    public int Id { get; set; }
    public int SalesSessionId { get; set; }
    
    [Required]
    public string SKU { get; set; } = string.Empty; // SKU da StoreInventory
    public int Quantity { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }
}