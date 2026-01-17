using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Services;

public class SellerService : ISellerService
{
    private readonly InventoryDbContext _context;

    public SellerService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Seller>> GetAllAsync()
    {
        return await _context.Sellers.ToListAsync();
    }

    public async Task<Seller?> GetByIdAsync(int id)
    {
        return await _context.Sellers.FindAsync(id);
    }

    public async Task<Seller> CreateAsync(Seller seller)
    {
        _context.Sellers.Add(seller);
        await _context.SaveChangesAsync();
        return seller;
    }
}