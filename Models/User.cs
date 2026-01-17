using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.API.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "Vendedor"; // Vendedor, Gerente, Coordenador

    public int? StoreId { get; set; }

    // Opcional: Relacionamento com a tabela de Sellers que você já possui
    public int? SellerId { get; set; }

    [ForeignKey("SellerId")]
    public Seller? Seller { get; set; }
}