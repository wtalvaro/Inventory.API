using Inventory.API.Models;

namespace Inventory.API.Services.Interfaces;

public interface ICartService
{
    Task<IEnumerable<CartItem>> GetItemsBySessionAsync(int sessionId);
    Task<CartItem> AddOrUpdateItemAsync(CartItem newItem);
    Task<bool> RemoveItemAsync(int id);
    Task<bool> ClearCartAsync(int sessionId);
}