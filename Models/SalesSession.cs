using System.ComponentModel.DataAnnotations;

namespace Inventory.API.Models;

public class SalesSession
{
    [Key]
    public int Id { get; set; }
    public int SellerId { get; set; }
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
    public decimal TotalOrderValue { get; set; }
    public string Status { get; set; } = "Open"; // Open, Finished, Abandoned (Desistência)
    public string? AbandonmentReason { get; set; } // Motivo da desistência
}