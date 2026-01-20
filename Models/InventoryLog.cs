using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Inventory.API.Models.Enums;

namespace Inventory.API.Models;

public class InventoryLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product? Product { get; set; }

    [Required]
    public int StoreId { get; set; }

    [ForeignKey("StoreId")]
    public Store? Store { get; set; }

    // Mantemos o SKU e Nome para histórico rápido (mesmo que o produto seja alterado)
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;

    [Required]
    public int QuantityChange { get; set; } // Ex: +10 (entrada), -2 (venda)

    [Required]
    public MovementType Type { get; set; } // Ex: "Ajuste Manual", "Venda", "Recebimento"

    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}