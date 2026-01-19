using Inventory.API.Data;
using Inventory.API.Models;
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
}