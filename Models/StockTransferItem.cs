namespace Inventory.API.Models;

public class StockTransferItem
{
    public int Id { get; set; }

    // Adicione esta linha:
    public int StockTransferId { get; set; }
    public StockTransfer StockTransfer { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int QuantitySent { get; set; }
    public int QuantityReceived { get; set; }

    public int OriginStockBatchId { get; set; }
}