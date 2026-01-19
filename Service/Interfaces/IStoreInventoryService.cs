using Inventory.API.Models;

namespace Inventory.API.Services.Interfaces;

public interface IStoreInventoryService
{
    // Consulta para o Vendedor (Filtra por loja) ou Coordenador (Geral)
    Task<IEnumerable<StoreInventory>> GetCatalogAsync(int storeId);

    Task<StoreInventory?> GetBySkuAsync(int storeId, string sku);

    // Alertas para o Setor de Compras
    Task<IEnumerable<StoreInventory>> GetItemsBelowMinimumAsync(int storeId);

    // Ajuste manual (Uso restrito a Gerentes)
    Task<StoreInventory?> AdjustStockAsync(int inventoryId, int newQuantity, string reason);
}