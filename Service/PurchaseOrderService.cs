using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Models.Enums;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Services;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly InventoryDbContext _context;

    public PurchaseOrderService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrder> CreateOrderAsync(PurchaseOrder order)
    {
        order.Status = OrderStatus.Pending;
        order.CreatedAt = DateTime.UtcNow;
        _context.PurchaseOrders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<PurchaseOrder?> GetOrderByIdAsync(int id)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Items)
            .ThenInclude(i => i.Product)
            .Include(po => po.Supplier)
            .FirstOrDefaultAsync(po => po.Id == id);
    }

    public async Task<bool> ReceiveOrderAsync(int orderId)
    {
        var order = await GetOrderByIdAsync(orderId);

        if (order == null || order.Status != OrderStatus.Pending)
            return false;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Atualizar Status da Ordem
            order.Status = OrderStatus.Received;
            order.ReceivedAt = DateTime.UtcNow;

            foreach (var item in order.Items)
            {
                // O DELTA aqui é positivo (Entrada de Compra)
                int delta = item.QuantityOrdered;

                // 2. Criar o Lote de Estoque (Rastreabilidade de Custo)
                var batch = new StockBatch
                {
                    ProductId = item.ProductId,
                    StoreId = order.StoreId,
                    PurchaseOrderId = order.Id,
                    InitialQuantity = delta,
                    CurrentQuantity = delta,
                    UnitCost = item.UnitCost,
                    EntryDate = DateTime.UtcNow
                };
                _context.StockBatches.Add(batch);

                // 3. Registrar no "Diário de Bordo" (InventoryLog)
                var log = new InventoryLog
                {
                    ProductId = item.ProductId,
                    StoreId = order.StoreId,
                    QuantityChange = delta,
                    Type = MovementType.Purchase, // <--- Vinculado ao nosso Enum
                    Notes = $"Entrada via OC #{order.Id} - Fornecedor ID: {order.SupplierId}",
                    CreatedAt = DateTime.UtcNow
                };
                _context.InventoryLogs.Add(log);

                // 4. Atualizar o saldo consolidado (StoreInventory)
                var inventory = await _context.StoreInventories
                    .FirstOrDefaultAsync(si => si.StoreId == order.StoreId && si.ProductId == item.ProductId);

                if (inventory != null)
                {
                    inventory.Quantity += delta;
                    inventory.LastUpdated = DateTime.UtcNow;
                }
            }

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

    public async Task<IEnumerable<PurchaseOrder>> GetOrdersByStoreAsync(int storeId)
    {
        return await _context.PurchaseOrders
            .Where(po => po.StoreId == storeId)
            .OrderByDescending(po => po.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PurchaseOrder>> GetAllOrdersAsync()
    {
        return await _context.PurchaseOrders
            .Include(po => po.Supplier) // Importante para aparecer o nome do fornecedor
            .Include(po => po.Store)    // Importante para aparecer a loja de destino
            .OrderByDescending(po => po.CreatedAt)
            .ToListAsync();
    }
}