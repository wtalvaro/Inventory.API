using System.Text.Json.Serialization;
using Inventory.API.Models.Enums;

namespace Inventory.API.Models;

public class SalesStep
{
    public int Id { get; set; }
    public int Second { get; set; }
    public string Message { get; set; } = string.Empty;
    public SalesStepType Type { get; set; } = SalesStepType.Rapport;

    // Tornamos o ProductId opcional (Nullable)
    public int? ProductId { get; set; }

    // Adicionamos filtros genéricos
    public string? Category { get; set; }
    public bool IsGlobal { get; set; } = false;

    [JsonIgnore]
    public virtual Product? Product { get; set; }
}