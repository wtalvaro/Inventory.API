using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Services;

public class SalesSessionService : ISalesSessionService
{
    private readonly InventoryDbContext _context;

    public SalesSessionService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<SalesSession> StartAsync(SalesSession session)
    {
        session.StartTime = DateTime.UtcNow;
        session.Status = "Open";
        session.Stage = "Discovery"; // Fase inicial padrão
        session.TotalOrderValue = 0;

        _context.SalesSessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<SalesSession?> GetByIdAsync(int id) =>
        await _context.SalesSessions.FindAsync(id);

    public async Task<bool> UpdateStageAsync(int id, string newStage)
    {
        var session = await _context.SalesSessions.FindAsync(id);
        if (session == null) return false;

        session.Stage = newStage;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal?> FinishAsync(int id)
    {
        var session = await _context.SalesSessions.FindAsync(id);
        if (session == null || session.Status != "Open") return null;

        // 1. Calcula o total baseado nos itens atuais do carrinho
        var total = await _context.CartItems
            .Where(c => c.SalesSessionId == id)
            .SumAsync(c => c.Quantity * c.UnitPrice);

        // 2. Atualiza os dados da sessão
        session.Status = "Finished";
        session.EndTime = DateTime.UtcNow;
        session.TotalOrderValue = total;

        // 3. Lógica de Gamificação: Atualiza o progresso do vendedor
        var seller = await _context.Sellers.FindAsync(session.SellerId);
        if (seller != null)
        {
            seller.CurrentSales += total;
        }

        await _context.SaveChangesAsync();
        return total;
    }

    public async Task<bool> AbandonAsync(int id, string reason)
    {
        var session = await _context.SalesSessions.FindAsync(id);
        if (session == null) return false;

        session.Status = "Abandoned";
        session.EndTime = DateTime.UtcNow;
        session.AbandonmentReason = reason;

        await _context.SaveChangesAsync();
        return true;
    }
}