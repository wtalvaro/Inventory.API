using Inventory.API.Models;

namespace Inventory.API.Services.Interfaces;

public interface ISalesStepService
{
    // Retorna a linha do tempo mesclando Global, Categoria e Produto
    Task<IEnumerable<SalesStep>> GetCoachTimelineAsync(int productId);

    // CRUD básico para gestão dos passos
    Task<SalesStep> CreateAsync(SalesStep step);
    Task<IEnumerable<SalesStep>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
}