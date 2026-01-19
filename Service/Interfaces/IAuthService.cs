using Inventory.API.Models;

namespace Inventory.API.Services.Interfaces;

public interface IAuthService
{
    // Retorna o usuário se as credenciais (Username + Senha pura) forem válidas
    Task<User?> ValidateUserAsync(string username, string password);

    // Gera o Token JWT (caso precise para Mobile/Integrações)
    string GenerateJwtToken(User user);
}