using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Inventory.API.Data;
using Inventory.API.Security;
using Inventory.API.Services;
using Inventory.API.Services.Interfaces;

namespace Inventory.API.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureConfig(this IServiceCollection services, IConfiguration config)
    {
        // 1. CORS: Permitir credenciais é vital para Cookies em chamadas Fetch
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(p => p
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials()); // OBRIGATÓRIO para evitar o erro 403/CORS com Cookies
        });

        // 2. Banco de Dados PostgreSQL
        var connectionString = config.GetConnectionString("InventoryDb")
            ?? throw new Exception("ConnectionString não encontrada!");

        services.AddDbContext<InventoryDbContext>(options =>
            options.UseNpgsql(connectionString));

        // 3. Controllers configurados para ignorar loops e formatar JSON corretamente para o JS
        services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

        // 4. Suporte ao Razor Pages (Onde o estado da sessão será verificado)
        services.AddRazorPages();

        // 5. Registro simplificado dos Serviços
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IStoreInventoryService, StoreInventoryService>();
        services.AddScoped<IInventoryLogService, InventoryLogService>();
        services.AddScoped<ISalesSessionService, SalesSessionService>();
        services.AddScoped<ISellerService, SellerService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISalesStepService, SalesStepService>();
        services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
        services.AddScoped<IPurchaseOrderItemService, PurchaseOrderItemService>();
        services.AddScoped<IStockBatchService, StockBatchService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IStockTransferService, StockTransferService>();
        services.AddScoped<IStockTransferItemService, StockTransferItemService>();

        return services;
    }

    public static IServiceCollection AddIdentityConfig(this IServiceCollection services, IConfiguration config)
    {
        // UNIFICADO: O Cookie é o esquema padrão para suportar o Razor
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.LoginPath = "/Index";
            options.Cookie.Name = "RetailProAuth";
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromHours(2);
            options.SlidingExpiration = true; // Renova o tempo enquanto o usuário usa
        })
        .AddJwtBearer(options =>
        {
            // Mantemos JWT apenas se houver integração externa (Mobile, etc)
            var jwtKey = config["Jwt:Key"] ?? "chave-secreta-muito-longa-e-segura";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };
        });

        services.AddHttpContextAccessor();

        // Políticas de Autorização baseadas em Roles (o que o Razor e as APIs usarão)
        // Mantendo as suas políticas de autorização originais
        services.AddAuthorizationBuilder()
            .AddPolicy("ApenasMinhaLoja", p => p.RequireRole("Gerente", "Coordenador", "Administrador").AddRequirements(new SameStoreRequirement()))
            .AddPolicy("ApenasCoordenador", p => p.RequireRole("Coordenador", "Administrador"))
            .AddPolicy("GerenteOuSuperior", p => p.RequireRole("Gerente", "Coordenador", "Administrador"))
            .AddPolicy("VendedorOuSuperior", p => p.RequireRole("Vendedor", "Estoquista", "Gerente", "Coordenador", "Administrador"));
        return services;
    }

    // Local: Inventory.API/Extensions/DependencyInjection.cs

    public static async Task InitDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<InventoryDbContext>();

            // 1. Aplica as migrações pendentes (Cria as tabelas se não existirem)
            await context.Database.MigrateAsync();

            // 2. Chama o Seeder para preencher os Usuários
            // Como o SeedUsers é 'async Task', usamos await
            await DbSeeder.SeedUsers(context);

            // 3. Chama o Seeder para os Passos de Venda
            // Como o SeedSalesSteps é 'void' (sincrono), chamamos direto
            DbSeeder.SeedSalesSteps(context);

            Console.WriteLine(">>> Banco de Dados inicializado e populado com sucesso!");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Erro ao inicializar o banco de dados.");
            throw; // Repassa o erro para sabermos o que falhou no console
        }
    }
}