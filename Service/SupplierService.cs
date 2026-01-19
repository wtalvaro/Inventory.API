using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Services;

public class SupplierService : ISupplierService
{
    private readonly InventoryDbContext _context;

    public SupplierService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync()
    {
        return await _context.Suppliers
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Supplier?> GetByIdAsync(int id)
    {
        return await _context.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Supplier?> GetByTaxIdAsync(string taxId)
    {
        return await _context.Suppliers
            .FirstOrDefaultAsync(s => s.TaxId == taxId);
    }

    public async Task<Supplier> CreateAsync(Supplier supplier)
    {
        // Validação: Documento Único
        var existing = await GetByTaxIdAsync(supplier.TaxId);
        if (existing != null)
            throw new InvalidOperationException("Já existe um fornecedor cadastrado com este documento.");

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();
        return supplier;
    }

    public async Task<bool> UpdateAsync(int id, Supplier supplier)
    {
        var existing = await _context.Suppliers.FindAsync(id);
        if (existing == null) return false;

        existing.Name = supplier.Name;
        existing.ContactEmail = supplier.ContactEmail;
        existing.Phone = supplier.Phone;
        existing.TaxId = supplier.TaxId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.PurchaseOrders)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (supplier == null) return false;

        // Regra de Negócio: Não deletamos fornecedores que já possuem compras no histórico
        if (supplier.PurchaseOrders.Any())
            throw new InvalidOperationException("Não é possível excluir um fornecedor com histórico de compras. Inative-o em vez de excluir.");

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();
        return true;
    }
}