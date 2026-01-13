using System.ComponentModel.DataAnnotations;

namespace Inventory.API.Models;

public class Seller
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public decimal DailyGoal { get; set; } // Meta do dia
    public decimal CurrentSales { get; set; } // Valor vendido hoje
    public bool IsActive { get; set; } = true;
}