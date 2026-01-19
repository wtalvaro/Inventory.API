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

        if (order == null || order.Status == OrderStatus.Received)
            return false;

        // 1. Atualizar Status da Ordem
        order.Status = OrderStatus.Received;
        order.ReceivedAt = DateTime.UtcNow;

        foreach (var item in order.Items)
        {
            // 2. Criar o Lote de Estoque (PEPS/FIFO)
            var batch = new StockBatch
            {
                ProductId = item.ProductId,
                StoreId = order.StoreId,
                PurchaseOrderId = order.Id,
                InitialQuantity = item.QuantityOrdered,
                CurrentQuantity = item.QuantityOrdered,
                UnitCost = item.UnitCost,
                EntryDate = DateTime.UtcNow
            };
            _context.StockBatches.Add(batch);

            // 3. Atualizar o saldo total na StoreInventory
            var inventory = await _context.StoreInventories
                .FirstOrDefaultAsync(si => si.StoreId == order.StoreId && si.ProductId == item.ProductId);

            if (inventory != null)
            {
                inventory.Quantity += item.QuantityOrdered;
                inventory.LastUpdated = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PurchaseOrder>> GetOrdersByStoreAsync(int storeId)
    {
        return await _context.PurchaseOrders
            .Where(po => po.StoreId == storeId)
            .OrderByDescending(po => po.CreatedAt)
            .ToListAsync();
    }
}