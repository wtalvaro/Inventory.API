using Inventory.API.Models;

namespace Inventory.API.Services.Interfaces;

public interface ISupplierService
{
    Task<IEnumerable<Supplier>> GetAllAsync();
    Task<Supplier?> GetByIdAsync(int id);
    Task<Supplier?> GetByTaxIdAsync(string taxId);
    Task<Supplier> CreateAsync(Supplier supplier);
    Task<bool> UpdateAsync(int id, Supplier supplier);
    Task<bool> DeleteAsync(int id);
}