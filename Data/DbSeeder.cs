using Inventory.API.Models;
// Certifique-se de que o namespace do Enum está correto aqui
using Inventory.API.Models.Enums;
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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123"), // Lembre-se de usar Hash no futuro!
                Role = UserRole.Gerente,    // Alterado de "Gerente"
                StoreId = 1
            },
            new User {
                Username = "vendedor.loja1",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Role = UserRole.Vendedor,     // Alterado de "Vendedor"
                StoreId = 1
            },
            new User {
                Username = "estoquista.loja1",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Role = UserRole.Estoquista, // Novo: Papel de Estoquista
                StoreId = 1
            },

            // LOJA 2
            new User {
                Username = "gerente.loja2",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
                Role = UserRole.Gerente,    // Alterado de "Gerente"
                StoreId = 2
            },

            // GLOBAL (ADMINISTRADOR/COORDENADOR)
            new User {
                Username = "coord.geral",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("coord123"),
                Role = UserRole.Coordenador, // Alterado de "Coordenador"
                StoreId = null // Usamos null para quem tem acesso global
            }
        };

        await context.Users.AddRangeAsync(testUsers);
        await context.SaveChangesAsync();
    }

    public static void SeedSalesSteps(InventoryDbContext context)
    {
        if (context.SalesSteps.Any()) return;

        context.SalesSteps.AddRange(
            new SalesStep
            {
                Second = 0,
                IsGlobal = true,
                Message = "Olá! Como posso ajudar você a encontrar o item perfeito hoje?",
                Type = SalesStepType.Rapport
            },
            new SalesStep
            {
                Second = 20,
                Category = "Camisas",
                Message = "Ofereça para provar e lembre de checar a disponibilidade de personalização.",
                Type = SalesStepType.Sondagem
            },
            new SalesStep
            {
                Second = 30,
                ProductId = 71,
                Message = "Destaque que esta é a tecnologia AEROREADY de jogo!",
                Type = SalesStepType.Fechamento
            }
        );

        context.SaveChanges();
    }
}