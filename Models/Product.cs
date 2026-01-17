using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.API.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = "Geral";
    public string SubCategory { get; set; } = string.Empty;
    // Segmentação de Mercado
    public string Gender { get; set; } = "Unissex";      // Masculino, Feminino, Unissex
    public string TargetAudience { get; set; } = "Adulto"; // Infantil, Juvenil, Adulto, Idoso
    // ESPECIFICAÇÕES GLOBAIS (JSONB) - Ex: Material, Garantia, Tecnologia
    [Column(TypeName = "jsonb")]
    public Dictionary<string, string> Specifications { get; set; } = new();
    // INTELIGÊNCIA DE VENDAS (JSONB)
    [Column(TypeName = "jsonb")]
    public Dictionary<string, string> SalesTips { get; set; } = new();

    [Column(TypeName = "jsonb")]
    public List<string> Benefits { get; set; } = new();

    [Column(TypeName = "jsonb")]
    public Dictionary<string, string> CrossSellAdvantages { get; set; } = new();

    [Column(TypeName = "jsonb")]
    public List<SalesStep>? SalesTimeline { get; set; } = new();

    public string[] RelatedSkus { get; set; } = Array.Empty<string>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}