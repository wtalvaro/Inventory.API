using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Inventory.API.Models.Enums;

namespace Inventory.API.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    // Alterado de string para o Enum UserRole
    [Required]
    public UserRole Role { get; set; } = UserRole.Vendedor;

    public int? StoreId { get; set; }

    // Relacionamento com o vendedor (se o usuário for um vendedor ativo)
    public int? SellerId { get; set; }

    [ForeignKey("SellerId")]
    public Seller? Seller { get; set; }

    [ForeignKey("StoreId")]
    public Store? Store { get; set; }
}