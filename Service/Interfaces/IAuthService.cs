using Inventory.API.Dtos;

namespace Inventory.API.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}