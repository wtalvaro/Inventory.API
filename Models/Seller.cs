using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.API.Models;

public class Seller
{
    [Key]
    public int Id { get; set; }
    public int StoreId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal DailyGoal { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentSales { get; set; }
    public bool IsActive { get; set; } = true;
}