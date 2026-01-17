using System.ComponentModel.DataAnnotations;

namespace Inventory.API.Models;

public class InventoryLog
{
    [Key]
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}