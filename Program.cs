using Inventory.API.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Altere de "DefaultConnection" para "InventoryDb"
var connectionString = builder.Configuration.GetConnectionString("InventoryDb");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("ERRO: A ConnectionString 'InventoryDb' não foi encontrada!");
}

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseNpgsql(dataSource));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(); // Registro do serviço (Correto)

var app = builder.Build();

// AJUSTE AQUI: No app.Environment, você MAPEA a rota, não registra o serviço.
// Configura o Scalar para ler o JSON gerado acima
if (app.Environment.IsDevelopment())
{
    // 1. GERA o endpoint do arquivo JSON (obrigatório)
    app.MapOpenApi();

    // 2. CRIA a interface visual que lê esse JSON
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Estoque API - Varejo")
               .WithTheme(ScalarTheme.Moon)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseAuthorization();
app.MapControllers();
app.UseDefaultFiles(); // Procura por index.html automaticamente
app.UseStaticFiles();  // Permite acessar arquivos na pasta wwwroot

app.Run();