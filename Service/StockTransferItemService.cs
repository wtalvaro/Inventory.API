using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Models.Enums;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Services;

public class StockTransferItemService : IStockTransferItemService
{
    private readonly InventoryDbContext _context;

    public StockTransferItemService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StockTransferItem>> GetByTransferIdAsync(int transferId)
    {
        return await _context.StockTransferItems
            .Include(i => i.Product)
            .Where(i => i.StockTransferId == transferId)
            .ToListAsync();
    }

    public async Task<StockTransferItem?> GetByIdAsync(int id)
    {
        return await _context.StockTransferItems
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<StockTransferItem> AddAsync(StockTransferItem item)
    {
        // Validação de Segurança: A transferência pai permite edições?
        var transfer = await _context.StockTransfers.FindAsync(item.StockTransferId);
        if (transfer == null || transfer.Status != TransferStatus.Pending)
            throw new InvalidOperationException("Não é possível adicionar itens a uma transferência que não está pendente.");

        _context.StockTransferItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> UpdateAsync(int id, StockTransferItem item)
    {
        var existing = await _context.StockTransferItems
            .Include(i => i.StockTransfer)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (existing == null || existing.StockTransfer.Status != TransferStatus.Pending)
            return false;

        existing.QuantitySent = item.QuantitySent;
        existing.ProductId = item.ProductId;
        existing.OriginStockBatchId = item.OriginStockBatchId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _context.StockTransferItems
            .Include(i => i.StockTransfer)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null || item.StockTransfer.Status != TransferStatus.Pending)
            return false;

        _context.StockTransferItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }
}