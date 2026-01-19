using Inventory.API.Models;

namespace Inventory.API.Services.Interfaces;

public interface IStockBatchService
{
    Task<IEnumerable<StockBatch>> GetBatchesByProductAsync(int storeId, int productId);
    Task<StockBatch?> GetByIdAsync(int id);
    // O método principal para a baixa inteligente (FIFO)
    Task<int> ReduceStockAsync(int storeId, int productId, int quantityToRemove);
    Task<bool> ExpireBatchAsync(int batchId);
}