using System.ComponentModel.DataAnnotations;

namespace Inventory.API.Models;

public class Store
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}