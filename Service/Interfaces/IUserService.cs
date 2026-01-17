using Inventory.API.Dtos;

namespace Inventory.API.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserReadDto>> GetAllAsync();
    Task<UserReadDto?> GetByIdAsync(int id);
    Task<UserReadDto> CreateAsync(UserCreateDto dto);
    Task<bool> UpdateAsync(int id, UserUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}