using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.API.Models;

public class SalesStep
{
    public int Second { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "info"; // info, alert, success
}

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string SKU { get; set; } = string.Empty;

    public string Category { get; set; } = "Geral";

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "jsonb")]
    public Dictionary<string, string> Specifications { get; set; } = new();

    public string Aisle { get; set; } = string.Empty;
    public string Shelf { get; set; } = string.Empty;
    public int LedPin { get; set; }
    public int LedDurationSeconds { get; set; } = 5;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string[] RelatedSkus { get; set; } = Array.Empty<string>();
    public string SalesTip { get; set; } = string.Empty;

    // Nova propriedade para o Roteiro Temporizado
    [Column(TypeName = "jsonb")]
    public List<SalesStep>? SalesTimeline { get; set; } = new();

    public string Benefits { get; set; } = string.Empty; // Benefícios do produto principal

    public string CrossSellAdvantage { get; set; } = string.Empty; // Vantagem ao ser vendido como combo
}