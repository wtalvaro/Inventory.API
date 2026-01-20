using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Inventory.API.Models;

namespace Inventory.API.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    // Tabelas Base
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Store> Stores { get; set; } = null!;
    public DbSet<StoreInventory> StoreInventories { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Seller> Sellers { get; set; } = null!;
    public DbSet<SalesSession> SalesSessions { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;
    public DbSet<SalesStep> SalesSteps { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;
    public DbSet<PurchaseOrderItem> PurchaseOrdersItems { get; set; } = null!;
    public DbSet<StockBatch> StockBatches { get; set; } = null!;
    public DbSet<InventoryLog> InventoryLogs { get; set; } = null!;
    public DbSet<StockTransfer> StockTransfers { get; set; } = null!;
    public DbSet<StockTransferItem> StockTransferItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region Configuração de Schemas e Tabelas

        // SCHEMA: Identity (Segurança e Pessoas)
        modelBuilder.Entity<User>().ToTable("Users", "Identity");
        modelBuilder.Entity<Seller>().ToTable("Sellers", "Identity");

        // SCHEMA: Catalog (Produtos e Definições)
        modelBuilder.Entity<Product>().ToTable("Products", "Catalog");
        modelBuilder.Entity<Supplier>().ToTable("Suppliers", "Catalog");
        modelBuilder.Entity<SalesStep>().ToTable("SalesSteps", "Catalog");

        // SCHEMA: Inventory (WMS e Logística)
        modelBuilder.Entity<Store>().ToTable("Stores", "Inventory");
        modelBuilder.Entity<StoreInventory>().ToTable("StoreInventories", "Inventory");
        modelBuilder.Entity<StockBatch>().ToTable("StockBatches", "Inventory");
        modelBuilder.Entity<InventoryLog>().ToTable("InventoryLogs", "Inventory");
        modelBuilder.Entity<StockTransfer>().ToTable("StockTransfers", "Inventory");
        modelBuilder.Entity<StockTransferItem>().ToTable("StockTransferItems", "Inventory");

        // SCHEMA: Sales (Operações Comerciais e Compras)
        modelBuilder.Entity<SalesSession>().ToTable("SalesSessions", "Sales");
        modelBuilder.Entity<CartItem>().ToTable("CartItems", "Sales");
        modelBuilder.Entity<PurchaseOrder>().ToTable("PurchaseOrders", "Sales");
        modelBuilder.Entity<PurchaseOrderItem>().ToTable("PurchaseOrdersItems", "Sales");

        #endregion

        #region Configurações de JSON e Conversores
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        var dictionaryComparer = new ValueComparer<Dictionary<string, string>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToDictionary(entry => entry.Key, entry => entry.Value));

        var listComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());
        #endregion

        // Configuração Product (JSONB e Relacionamentos)
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Specifications).HasColumnType("jsonb").HasConversion(v => JsonSerializer.Serialize(v, jsonOptions), v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, jsonOptions) ?? new()).Metadata.SetValueComparer(dictionaryComparer);
            entity.Property(p => p.SalesTips).HasColumnType("jsonb").HasConversion(v => JsonSerializer.Serialize(v, jsonOptions), v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, jsonOptions) ?? new()).Metadata.SetValueComparer(dictionaryComparer);
            entity.Property(p => p.Benefits).HasColumnType("jsonb").HasConversion(v => JsonSerializer.Serialize(v, jsonOptions), v => JsonSerializer.Deserialize<List<string>>(v, jsonOptions) ?? new()).Metadata.SetValueComparer(listComparer);
            entity.Property(p => p.CrossSellAdvantages).HasColumnType("jsonb").HasConversion(v => JsonSerializer.Serialize(v, jsonOptions), v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, jsonOptions) ?? new()).Metadata.SetValueComparer(dictionaryComparer);
        });

        // Configuração StoreInventory
        modelBuilder.Entity<StoreInventory>(entity =>
        {
            entity.Property(si => si.VariantData).HasColumnType("jsonb").HasConversion(v => JsonSerializer.Serialize(v, jsonOptions), v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, jsonOptions) ?? new()).Metadata.SetValueComparer(dictionaryComparer);
            entity.HasIndex(si => new { si.StoreId, si.SKU }).IsUnique();

            entity.HasOne(si => si.Product)
                .WithMany()
                .HasForeignKey(si => si.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração SalesStep (Hierarquia e Performance)
        modelBuilder.Entity<SalesStep>(entity =>
        {
            entity.HasOne(s => s.Product)
                .WithMany(p => p.SalesTimeline)
                .HasForeignKey(s => s.ProductId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(s => s.Category).HasFilter("\"Category\" IS NOT NULL");
            entity.HasIndex(s => s.IsGlobal).HasFilter("\"IsGlobal\" = TRUE");
        });

        // Configurações de Compras e Estoque (Lotes e Fornecedores)
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasIndex(s => s.TaxId).IsUnique(); // CNPJ Único
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasOne(po => po.Supplier)
                .WithMany(s => s.PurchaseOrders)
                .HasForeignKey(po => po.SupplierId);
        });

        modelBuilder.Entity<StockBatch>(entity =>
        {
            entity.HasOne(sb => sb.Product)
                .WithMany()
                .HasForeignKey(sb => sb.ProductId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Role)
                  .HasConversion<string>() // Esta é a "mágica" que resolve o problema
                  .HasMaxLength(20);

            entity.HasIndex(u => u.Username).IsUnique();
        });

        modelBuilder.Entity<StockTransfer>(entity =>
        {
            entity.HasOne(st => st.OriginStore)
                .WithMany()
                .HasForeignKey(st => st.OriginStoreId)
                .OnDelete(DeleteBehavior.Restrict); // Evita ciclos de cascata

            entity.HasOne(st => st.DestinationStore)
                .WithMany()
                .HasForeignKey(st => st.DestinationStoreId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(st => st.Status)
                  .HasConversion<string>(); // Salva como texto no DB para auditoria
        });

        modelBuilder.Entity<StockTransferItem>(entity =>
        {
            entity.HasOne(sti => sti.StockTransfer)
                .WithMany(st => st.Items)
                .HasForeignKey(sti => sti.StockTransferId);

            entity.HasOne(sti => sti.Product)
                .WithMany()
                .HasForeignKey(sti => sti.ProductId);
        });

        modelBuilder.Entity<Product>()
            .HasMany(p => p.Suppliers)
            .WithMany(s => s.Products)
            .UsingEntity<Dictionary<string, object>>(
                "ProductSupplier", // Nome da tabela que será criada no banco
                j => j.HasOne<Supplier>().WithMany().HasForeignKey("SupplierId"),
                j => j.HasOne<Product>().WithMany().HasForeignKey("ProductId")
            );

        #region Conversão de Enums para String

        // 1. Usuários: Armazena "Gerente", "Administrador", etc.
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Role)
                  .HasConversion<string>()
                  .HasMaxLength(30);
        });

        // 2. Ordens de Compra: Armazena "Draft", "Shipped", etc.
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.Property(p => p.Status)
                  .HasConversion<string>()
                  .HasMaxLength(30);
        });

        // 3. Passos de Venda: Armazena "Rapport", "Sondagem", etc.
        modelBuilder.Entity<SalesStep>(entity =>
        {
            entity.Property(s => s.Type)
                  .HasConversion<string>()
                  .HasMaxLength(30);
        });

        // 4. Transferências: Armazena "InTransit", "Divergent", etc.
        modelBuilder.Entity<StockTransfer>(entity =>
        {
            entity.Property(st => st.Status)
                  .HasConversion<string>()
                  .HasMaxLength(30);
        });

        modelBuilder.Entity<InventoryLog>(entity =>
        {
            entity.Property(l => l.Type)
                  .HasConversion<string>() // Salva "Sale" em vez de 1
                  .HasMaxLength(50);
        });

        #endregion

        // Aplicar Precisão Decimal Centralizada
        ConfigureDecimalPrecision(modelBuilder);
    }

    private void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
    {
        // Vendas e Metas
        modelBuilder.Entity<Seller>().Property(s => s.DailyGoal).HasPrecision(18, 2);
        modelBuilder.Entity<Seller>().Property(s => s.CurrentSales).HasPrecision(18, 2);
        modelBuilder.Entity<SalesSession>().Property(ss => ss.TotalOrderValue).HasPrecision(18, 2);
        modelBuilder.Entity<CartItem>().Property(ci => ci.UnitPrice).HasPrecision(18, 2);

        // Inventário e Preços Locais
        modelBuilder.Entity<StoreInventory>().Property(si => si.LocalPrice).HasPrecision(18, 2);

        // Compras e Custos (Novas Tabelas)
        modelBuilder.Entity<PurchaseOrderItem>().Property(p => p.UnitCost).HasPrecision(18, 2);
        modelBuilder.Entity<PurchaseOrder>().Property(p => p.TotalCost).HasPrecision(18, 2);
        modelBuilder.Entity<PurchaseOrder>().Property(p => p.ServiceFee).HasPrecision(18, 2);
        modelBuilder.Entity<PurchaseOrder>().Property(p => p.TaxAmount).HasPrecision(18, 2);
        modelBuilder.Entity<PurchaseOrder>().Property(p => p.ShippingCost).HasPrecision(18, 2);
        modelBuilder.Entity<StockBatch>().Property(p => p.UnitCost).HasPrecision(18, 2);
        modelBuilder.Entity<Supplier>().Property(s => s.BaseOrderFee).HasPrecision(18, 2);
        modelBuilder.Entity<Supplier>().Property(s => s.DefaultShippingFee).HasPrecision(18, 2);
    }
}