using System.ComponentModel.DataAnnotations;

namespace Inventory.API.Models;

public class SalesStep
{
    public int Second { get; set; }
    [Required]
    public string Message { get; set; } = string.Empty;
    public SalesStepType Type { get; set; } = SalesStepType.Info;
}