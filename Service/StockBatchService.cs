using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Models.Enums;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Services;

public class StockBatchService : IStockBatchService
{
    private readonly InventoryDbContext _context;

    public StockBatchService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StockBatch>> GetBatchesByProductAsync(int storeId, int productId)
    {
        return await _context.StockBatches
            .Where(b => b.StoreId == storeId && b.ProductId == productId && b.CurrentQuantity > 0)
            .OrderBy(b => b.EntryDate) // Garante a ordem cronológica
            .ToListAsync();
    }

    public async Task<StockBatch?> GetByIdAsync(int id)
    {
        return await _context.StockBatches
            .Include(b => b.Product)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<int> ReduceStockAsync(int storeId, int productId, int quantityToRemove)
    {
        // 1. Busca lotes disponíveis ordenados pela data de entrada (FIFO)
        var batches = await _context.StockBatches
            .Where(b => b.StoreId == storeId && b.ProductId == productId && b.CurrentQuantity > 0)
            .OrderBy(b => b.EntryDate)
            .ToListAsync();

        int remainingToRemove = quantityToRemove;

        foreach (var batch in batches)
        {
            if (remainingToRemove <= 0) break;

            if (batch.CurrentQuantity >= remainingToRemove)
            {
                // Este lote supre toda a necessidade restante
                batch.CurrentQuantity -= remainingToRemove;
                remainingToRemove = 0;
            }
            else
            {
                // Este lote não é suficiente, esvaziamos ele e passamos para o próximo
                remainingToRemove -= batch.CurrentQuantity;
                batch.CurrentQuantity = 0;
            }
        }

        await _context.SaveChangesAsync();

        // Retorna quanto ainda faltou remover (se > 0, indica ruptura de estoque)
        return remainingToRemove;
    }

    public async Task<bool> ExpireBatchAsync(int batchId)
    {
        var batch = await _context.StockBatches.FindAsync(batchId);
        if (batch == null) return false;

        // Se o lote venceu, removemos a quantidade disponível dele
        batch.CurrentQuantity = 0;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RecordMovementAsync(int productId, int storeId, int quantity, MovementType type, string reason)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var product = await _context.Products.FindAsync(productId);
            var inventory = await _context.StoreInventories
                .FirstOrDefaultAsync(si => si.ProductId == productId && si.StoreId == storeId);

            if (product == null || inventory == null) return false;

            // 1. CRIAR O LOG (O "Cartório" da movimentação - Imutável)
            var log = new InventoryLog
            {
                ProductId = productId,
                StoreId = storeId,
                SKU = inventory.SKU,
                ProductName = product.Name,
                QuantityChange = quantity,
                Type = type,
                Notes = reason,
                CreatedAt = DateTime.UtcNow
            };
            _context.InventoryLogs.Add(log);

            // 2. LÓGICA ERP: Se for Saída (quantidade negativa), fazemos a baixa FIFO nos Lotes
            if (quantity < 0)
            {
                int toReduce = Math.Abs(quantity);
                var batches = await _context.StockBatches
                    .Where(b => b.ProductId == productId && b.StoreId == storeId && b.CurrentQuantity > 0)
                    .OrderBy(b => b.EntryDate) // Primeiro que entra, primeiro que sai
                    .ToListAsync();

                foreach (var batch in batches)
                {
                    if (toReduce <= 0) break;

                    int take = Math.Min(batch.CurrentQuantity, toReduce);
                    batch.CurrentQuantity -= take;
                    toReduce -= take;
                }

                if (toReduce > 0) throw new Exception("Estoque insuficiente nos lotes para esta saída.");
            }

            // 3. ATUALIZAR O SALDO ESPELHO (StoreInventory)
            // O saldo aqui é apenas um reflexo consolidado dos lotes e logs
            inventory.Quantity += quantity;
            inventory.LastUpdated = DateTime.UtcNow;

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