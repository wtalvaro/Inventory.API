using Inventory.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Data;

public static class DbSeeder
{
    public static async Task SeedUsers(InventoryDbContext context)
    {
        // Só adiciona se o banco estiver vazio para não duplicar
        if (await context.Users.AnyAsync()) return;

        var testUsers = new List<User>
        {
            // LOJA 1
            new User {
                Username = "gerente.loja1",
                PasswordHash = "senha123",
                Role = "Gerente",
                StoreId = 1
            },
            new User {
                Username = "vendedor.loja1",
                PasswordHash = "senha123",
                Role = "Vendedor",
                StoreId = 1
            },

            // LOJA 2
            new User {
                Username = "gerente.loja2",
                PasswordHash = "senha123",
                Role = "Gerente",
                StoreId = 2
            },

            // GLOBAL (COORDENADOR)
            new User {
                Username = "admin.geral",
                PasswordHash = "admin123",
                Role = "Coordenador",
                StoreId = 0 // Coordenadores geralmente ignoram a trava de StoreId
            }
        };

        await context.Users.AddRangeAsync(testUsers);
        await context.SaveChangesAsync();
    }
}