using Inventory.API.Models;

namespace Inventory.API.Services.Interfaces;

public interface ISalesSessionService
{
    Task<SalesSession> StartAsync(SalesSession session);
    Task<SalesSession?> GetByIdAsync(int id);
    Task<bool> UpdateStageAsync(int id, string newStage);
    Task<decimal?> FinishAsync(int id); // Retorna o total da venda se sucesso
    Task<bool> AbandonAsync(int id, string reason);
}