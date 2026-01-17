using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Services;

public class CartService : ICartService
{
    private readonly InventoryDbContext _context;

    public CartService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CartItem>> GetItemsBySessionAsync(int sessionId)
    {
        return await _context.CartItems
            .Where(c => c.SalesSessionId == sessionId)
            .ToListAsync();
    }

    public async Task<CartItem> AddOrUpdateItemAsync(CartItem newItem)
    {
        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.SalesSessionId == newItem.SalesSessionId && c.SKU == newItem.SKU);

        if (existingItem != null)
        {
            existingItem.Quantity += newItem.Quantity;
            // Opcional: existingItem.UnitPrice = newItem.UnitPrice;
        }
        else
        {
            _context.CartItems.Add(newItem);
        }

        await _context.SaveChangesAsync();
        return existingItem ?? newItem;
    }

    public async Task<bool> RemoveItemAsync(int id)
    {
        var item = await _context.CartItems.FindAsync(id);
        if (item == null) return false;

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ClearCartAsync(int sessionId)
    {
        var items = await _context.CartItems
            .Where(c => c.SalesSessionId == sessionId)
            .ToListAsync();

        if (!items.Any()) return true;

        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
        return true;
    }
}