using Inventory.API.Models;

namespace Inventory.API.Services.Interfaces;

public interface ISellerService
{
    Task<IEnumerable<Seller>> GetAllAsync();
    Task<Seller?> GetByIdAsync(int id);
    Task<Seller> CreateAsync(Seller seller);
}