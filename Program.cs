using Inventory.API.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURAÇÃO DE SERVIÇOS ---

// Configura Infraestrutura: DB, CORS, NewtonsoftJson e Injeção dos Services (IInventoryService, etc)
builder.Services.AddInfrastructureConfig(builder.Configuration);

// Configura Identidade: Cookies (Padrão para Razor) + JWT (API) e Políticas de Acesso
builder.Services.AddIdentityConfig(builder.Configuration);

// Documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// --- 2. PIPELINE DE EXECUÇÃO (Middlewares) ---

// Inicializa o banco de dados (Migrations + Seeders)
await app.InitDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("RetailPro API").WithTheme(ScalarTheme.Moon);
    });
}

// Arquivos Estáticos: Essencial para carregar o seu app.js e app.css da wwwroot
app.UseStaticFiles();

app.UseRouting();

// CORS deve vir antes da Autenticação se o JS for consumir de origens diferentes
app.UseCors();

// ORDEM VITAL: Quem é o usuário? -> O que ele pode fazer?
app.UseAuthentication();
app.UseAuthorization();

// --- 3. MAPEAMENTO DE ROTAS ---

// Prioridade: Razor Pages (Sua Interface Principal e Gestão de Estado)
app.MapRazorPages();

// API: Endpoints para o Fetch do JavaScript (Dinamismo da Página)
app.MapControllers();

app.Run();