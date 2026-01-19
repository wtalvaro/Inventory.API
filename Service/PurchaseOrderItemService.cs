using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Models.Enums;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Services;

public class PurchaseOrderItemService : IPurchaseOrderItemService
{
    private readonly InventoryDbContext _context;

    public PurchaseOrderItemService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrderItem> AddItemToOrderAsync(PurchaseOrderItem item)
    {
        // Verifica se a ordem permite edições
        var order = await _context.PurchaseOrders.FindAsync(item.PurchaseOrderId);
        if (order == null || order.Status != OrderStatus.Draft)
            throw new InvalidOperationException("Apenas ordens em Rascunho podem receber novos itens.");

        _context.PurchaseOrdersItems.Add(item);

        // Atualiza o custo total da ordem automaticamente
        order.TotalCost += (item.QuantityOrdered * item.UnitCost);

        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> UpdateItemAsync(int id, int quantity, decimal unitCost)
    {
        var item = await _context.PurchaseOrdersItems
            .Include(i => i.PurchaseOrder)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null || item.PurchaseOrder.Status != OrderStatus.Draft)
            return false;

        // Ajusta o TotalCost da Ordem (Subtrai o antigo, soma o novo)
        item.PurchaseOrder.TotalCost -= (item.QuantityOrdered * item.UnitCost);

        item.QuantityOrdered = quantity;
        item.UnitCost = unitCost;

        item.PurchaseOrder.TotalCost += (quantity * unitCost);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveItemAsync(int id)
    {
        var item = await _context.PurchaseOrdersItems
            .Include(i => i.PurchaseOrder)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null || item.PurchaseOrder.Status != OrderStatus.Draft)
            return false;

        item.PurchaseOrder.TotalCost -= (item.QuantityOrdered * item.UnitCost);
        _context.PurchaseOrdersItems.Remove(item);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PurchaseOrderItem>> GetItemsByOrderIdAsync(int orderId)
    {
        return await _context.PurchaseOrdersItems
            .Include(i => i.Product)
            .Where(i => i.PurchaseOrderId == orderId)
            .ToListAsync();
    }
}