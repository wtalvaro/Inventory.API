using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Services;

public class SalesStepService : ISalesStepService
{
    private readonly InventoryDbContext _context;

    public SalesStepService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SalesStep>> GetCoachTimelineAsync(int productId)
    {
        var product = await _context.Products
            .Select(p => new { p.Id, p.Category })
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null) return Enumerable.Empty<SalesStep>();

        // Busca todos os candidatos (Global, Categoria ou o Produto específico)
        var allSteps = await _context.SalesSteps
            .Where(s => s.IsGlobal
                     || s.Category == product.Category
                     || s.ProductId == productId)
            .ToListAsync();

        // Lógica de Prioridade: Produto (0) > Categoria (1) > Global (2)
        // Isso resolve o conflito se houver dois passos para o mesmo segundo
        var timeline = allSteps
            .GroupBy(s => s.Second)
            .Select(g => g
                .OrderBy(s => s.ProductId.HasValue ? 0 : (s.Category != null ? 1 : 2))
                .First())
            .OrderBy(s => s.Second)
            .ToList();

        return timeline;
    }

    public async Task<SalesStep> CreateAsync(SalesStep step)
    {
        _context.SalesSteps.Add(step);
        await _context.SaveChangesAsync();
        return step;
    }

    public async Task<IEnumerable<SalesStep>> GetAllAsync()
    {
        return await _context.SalesSteps.OrderBy(s => s.Second).ToListAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var step = await _context.SalesSteps.FindAsync(id);
        if (step == null) return false;

        _context.SalesSteps.Remove(step);
        await _context.SaveChangesAsync();
        return true;
    }
}