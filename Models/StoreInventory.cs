using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.API.Models;

public class StoreInventory
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int StoreId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public string SKU { get; set; } = string.Empty;

    // VARIANTES DA LOJA (JSONB) - Ex: {"Cor": "Vermelho", "Tamanho": "42"}
    [Column(TypeName = "jsonb")]
    public Dictionary<string, string> VariantData { get; set; } = new();

    // PERFORMANCE OPERACIONAL (Colunas Tipadas)
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LocalPrice { get; set; }

    public string Aisle { get; set; } = string.Empty;
    public string Shelf { get; set; } = string.Empty;

    // Teorias Modernas:
    public int MinimumStock { get; set; } // Se atingir isso, o sistema avisa o Compras
    public int SafetyStock { get; set; }  // "Reserva de emergência"

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // Propriedade de Navegação
    [ForeignKey("ProductId")]
    public Product? Product { get; set; }

    [ForeignKey("StoreId")]
    public Store? Store { get; set; }
}