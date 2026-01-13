namespace Inventory.API.Models;

public class InventoryLog
{
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int DurationUsed { get; set; } // O tempo que o LED ficou aceso
}