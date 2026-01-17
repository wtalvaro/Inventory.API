using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.API.Models;

public class SalesSession
{
    [Key]
    public int Id { get; set; }
    public int StoreId { get; set; }
    public int SellerId { get; set; }
    public int? CustomerId { get; set; } 

    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
    
    public string Status { get; set; } = "Open"; // Open, Finished, Abandoned
    public string Stage { get; set; } = "Discovery"; // Funil: Discovery, Trial, Closing
    
    public string? AbandonmentReason { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalOrderValue { get; set; }
}