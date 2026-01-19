using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Models.Enums;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Services;

public class StockTransferService : IStockTransferService
{
    private readonly InventoryDbContext _context;

    public StockTransferService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StockTransfer>> GetAllAsync()
    {
        return await _context.StockTransfers
            .Include(t => t.OriginStore)
            .Include(t => t.DestinationStore)
            .Include(t => t.Items)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<StockTransfer?> GetByIdAsync(int id)
    {
        return await _context.StockTransfers
            .Include(t => t.OriginStore)
            .Include(t => t.DestinationStore)
            .Include(t => t.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<StockTransfer> CreateAsync(StockTransfer transfer)
    {
        transfer.Status = TransferStatus.Pending;
        transfer.CreatedAt = DateTime.UtcNow;

        _context.StockTransfers.Add(transfer);
        await _context.SaveChangesAsync();
        return transfer;
    }

    public async Task<bool> ShipAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var transfer = await _context.StockTransfers.Include(t => t.Items).FirstOrDefaultAsync(t => t.Id == id);
            if (transfer == null || transfer.Status != TransferStatus.Pending) return false;

            foreach (var item in transfer.Items)
            {
                var batch = await _context.StockBatches.FindAsync(item.OriginStockBatchId);

                // AJUSTE: Usando 'CurrentQuantity' em vez de 'Quantity'
                if (batch == null || batch.CurrentQuantity < item.QuantitySent)
                    throw new Exception($"Estoque insuficiente no lote {item.OriginStockBatchId}");

                batch.CurrentQuantity -= item.QuantitySent;

                var inventory = await _context.StoreInventories
                    .FirstOrDefaultAsync(si => si.StoreId == transfer.OriginStoreId && si.ProductId == item.ProductId);
                if (inventory != null) inventory.Quantity -= item.QuantitySent;
            }

            transfer.Status = TransferStatus.InTransit;
            transfer.ShippedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> ReceiveAsync(int id, List<StockTransferItem> receivedItems, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var transfer = await _context.StockTransfers.Include(t => t.Items).FirstOrDefaultAsync(t => t.Id == id);
            if (transfer == null || transfer.Status != TransferStatus.InTransit) return false;

            bool hasDivergence = false;

            foreach (var item in transfer.Items)
            {
                var received = receivedItems.FirstOrDefault(ri => ri.Id == item.Id);
                item.QuantityReceived = received?.QuantityReceived ?? 0;

                if (item.QuantityReceived != item.QuantitySent) hasDivergence = true;

                var originBatch = await _context.StockBatches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == item.OriginStockBatchId);

                // AJUSTE: Alinhado com as propriedades de StockBatch.cs
                var newBatch = new StockBatch
                {
                    ProductId = item.ProductId,
                    StoreId = transfer.DestinationStoreId,
                    InitialQuantity = item.QuantityReceived, // Define a quantidade inicial do novo lote
                    CurrentQuantity = item.QuantityReceived, // Define o saldo atual
                    UnitCost = originBatch?.UnitCost ?? 0,
                    EntryDate = DateTime.UtcNow,             // Em vez de ReceivedDate
                    PurchaseOrderId = originBatch?.PurchaseOrderId // Mantém o link com a compra original
                };
                _context.StockBatches.Add(newBatch);

                var inventory = await _context.StoreInventories
                    .FirstOrDefaultAsync(si => si.StoreId == transfer.DestinationStoreId && si.ProductId == item.ProductId);

                if (inventory != null) inventory.Quantity += item.QuantityReceived;
            }

            transfer.Status = hasDivergence ? TransferStatus.Divergent : TransferStatus.Received;
            transfer.ReceivedAt = DateTime.UtcNow;
            transfer.ReceivedByUserId = userId;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }
}