using Microsoft.EntityFrameworkCore;
using Inventory.API.Models;

namespace Inventory.API.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
    }

    // Esta é a tabela que será criada no PostgreSQL
    public DbSet<Product> Products { get; set; }
    public DbSet<InventoryLog> InventoryLogs { get; set; }
    // Novas tabelas para o módulo Vendedor
    public DbSet<Seller> Sellers { get; set; }
    public DbSet<SalesSession> SalesSessions { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().Property(b => b.Specifications).HasColumnType("jsonb");
        modelBuilder.Entity<Product>().HasIndex(u => u.SKU).IsUnique();

        // Configuração adicional para precisão decimal
        modelBuilder.Entity<Seller>().Property(s => s.DailyGoal).HasPrecision(18, 2);
        modelBuilder.Entity<SalesSession>().Property(ss => ss.TotalOrderValue).HasPrecision(18, 2);
        modelBuilder.Entity<CartItem>().Property(ci => ci.UnitPrice).HasPrecision(18, 2);
    }
}