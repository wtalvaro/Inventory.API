using Inventory.API.Data;
using Inventory.API.Models;
using Inventory.API.Dtos;
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
        var users = await _context.Users.ToListAsync();
        return users.Select(u => new UserReadDto(u.Id, u.Username, u.Role, u.StoreId));
    }

    public async Task<UserReadDto?> GetByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        return new UserReadDto(user.Id, user.Username, user.Role, user.StoreId);
    }

    public async Task<UserReadDto> CreateAsync(UserCreateDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            Role = dto.Role,
            StoreId = dto.StoreId,
            PasswordHash = dto.Password // Lembre-se de usar Hash em produção!
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserReadDto(user.Id, user.Username, user.Role, user.StoreId);
    }

    public async Task<bool> UpdateAsync(int id, UserUpdateDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        user.Username = dto.Username;
        user.Role = dto.Role;
        user.StoreId = dto.StoreId;

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
}