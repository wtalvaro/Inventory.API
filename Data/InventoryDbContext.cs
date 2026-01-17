using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking; // Necessário para ValueComparer
using Inventory.API.Models;

namespace Inventory.API.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Store> Stores { get; set; } = null!;
    public DbSet<StoreInventory> StoreInventories { get; set; } = null!;
    public DbSet<Seller> Sellers { get; set; } = null!;
    public DbSet<SalesSession> SalesSessions { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;
    public DbSet<InventoryLog> InventoryLogs { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Configurações Globais de JSON
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        // 2. Definição de Comparadores (Value Comparers)
        // Isso resolve o erro "property is a collection type with no value comparer"
        
        var dictionaryComparer = new ValueComparer<Dictionary<string, string>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToDictionary(entry => entry.Key, entry => entry.Value));

        var listComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        // 3. Configuração da Entidade Product (Campos JSONB)
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Specifications)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, jsonOptions) ?? new())
                .Metadata.SetValueComparer(dictionaryComparer); // Aplica o comparador

            entity.Property(p => p.SalesTips)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, jsonOptions) ?? new())
                .Metadata.SetValueComparer(dictionaryComparer);

            entity.Property(p => p.Benefits)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, jsonOptions) ?? new())
                .Metadata.SetValueComparer(listComparer);

            entity.Property(p => p.CrossSellAdvantages)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, jsonOptions) ?? new())
                .Metadata.SetValueComparer(dictionaryComparer);

            entity.Property(p => p.SalesTimeline)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => v == null ? null : JsonSerializer.Serialize(v, jsonOptions),
                    v => string.IsNullOrEmpty(v) ? null : JsonSerializer.Deserialize<List<SalesStep>>(v, jsonOptions));
        });

        // 4. Configuração da Entidade StoreInventory
        modelBuilder.Entity<StoreInventory>(entity =>
        {
            entity.Property(si => si.VariantData)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, jsonOptions) ?? new())
                .Metadata.SetValueComparer(dictionaryComparer);

            entity.HasIndex(si => new { si.StoreId, si.SKU }).IsUnique();
            
            entity.Property(si => si.LocalPrice).HasPrecision(18, 2);
        });

        // 5. Configuração de Precisão Decimal Restante
        ConfigureDecimalPrecision(modelBuilder);

        // 6. Configuração de Relacionamentos e Índices
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        modelBuilder.Entity<StoreInventory>()
            .HasOne(si => si.Product).WithMany().HasForeignKey(si => si.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Seller>().Property(s => s.DailyGoal).HasPrecision(18, 2);
        modelBuilder.Entity<Seller>().Property(s => s.CurrentSales).HasPrecision(18, 2);
        modelBuilder.Entity<SalesSession>().Property(ss => ss.TotalOrderValue).HasPrecision(18, 2);
        modelBuilder.Entity<CartItem>().Property(ci => ci.UnitPrice).HasPrecision(18, 2);
    }
}