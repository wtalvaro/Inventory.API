using Inventory.API.Models;

namespace Inventory.API.Services.Interfaces;

public interface IStockTransferService
{
    Task<IEnumerable<StockTransfer>> GetAllAsync();
    Task<StockTransfer?> GetByIdAsync(int id);
    Task<StockTransfer> CreateAsync(StockTransfer transfer);
    Task<bool> ShipAsync(int id); // Marca como InTransit e retira do estoque de origem
    Task<bool> ReceiveAsync(int id, List<StockTransferItem> receivedItems, int userId); // Finaliza e entra no destino
}