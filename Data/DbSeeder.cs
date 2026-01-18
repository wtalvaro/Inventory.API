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

    public static void SeedSalesSteps(InventoryDbContext context)
    {
        if (context.SalesSteps.Any()) return; // Evita duplicar se já houver dados

        context.SalesSteps.AddRange(
            // 1. Passos Globais (Aparecem para todos)
            new SalesStep
            {
                Second = 0,
                IsGlobal = true,
                Message = "Olá! Como posso ajudar você a encontrar o item perfeito hoje?",
                Type = SalesStepType.Rapport
            },

            // 2. Passos por Categoria (Todas as Camisas)
            new SalesStep
            {
                Second = 20,
                Category = "Camisas",
                Message = "Ofereça para provar e lembre de checar a disponibilidade de personalização.",
                Type = SalesStepType.Sondagem
            },

            // 3. Passo Específico (Ex: Camisa do Flamengo ID 71)
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