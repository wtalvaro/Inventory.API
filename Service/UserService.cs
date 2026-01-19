using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Models.Dtos;
using Inventory.API.Models.Enums;
using Inventory.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Services;

public class UserService : IUserService
{
    private readonly InventoryDbContext _context;

    public UserService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserReadDto>> GetAllAsync()
    {
        var users = await _context.Users.AsNoTracking().ToListAsync();
        return users.Select(MapToReadDto);
    }

    public async Task<UserReadDto?> GetByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user != null ? MapToReadDto(user) : null;
    }

    public async Task<UserReadDto> CreateAsync(UserCreateDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            StoreId = dto.StoreId,
            // Converte a string do DTO para o Enum do Modelo
            Role = Enum.Parse<UserRole>(dto.Role),
            // Segurança: Nunca salvar senha em texto limpo
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return MapToReadDto(user);
    }

    public async Task<bool> UpdateAsync(int id, UserUpdateDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        user.Username = dto.Username;
        user.StoreId = dto.StoreId;
        user.Role = Enum.Parse<UserRole>(dto.Role);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    // Helper para transformar o Modelo em DTO de leitura
    private static UserReadDto MapToReadDto(User user) =>
        new UserReadDto(user.Id, user.Username, user.Role.ToString(), user.StoreId);
}