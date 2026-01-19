using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Inventory.API.Models.Enums;

namespace Inventory.API.Models;

public class StockTransfer
{
    [Key]
    public int Id { get; set; }

    public int OriginStoreId { get; set; }
    [ForeignKey("OriginStoreId")]
    public Store OriginStore { get; set; } = null!;

    public int DestinationStoreId { get; set; }
    [ForeignKey("DestinationStoreId")]
    public Store DestinationStore { get; set; } = null!;

    [Required]
    public TransferStatus Status { get; set; } = TransferStatus.Pending;

    // Auditoria: Quem enviou e quem recebeu
    public int CreatedByUserId { get; set; }
    public int? ReceivedByUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ShippedAt { get; set; }
    public DateTime? ReceivedAt { get; set; }

    public List<StockTransferItem> Items { get; set; } = new();
}