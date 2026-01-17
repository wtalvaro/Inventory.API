using Inventory.API.Models;

namespace Inventory.API.Services.Interfaces;

public interface IStoreService
{
    Task<IEnumerable<Store>> GetAllActiveAsync();
    Task<Store?> GetByIdAsync(int id);
    Task<Store> CreateAsync(Store store);
    Task<bool> DeactivateAsync(int id);
}