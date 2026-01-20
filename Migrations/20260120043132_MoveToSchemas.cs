using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.API.Migrations
{
    /// <inheritdoc />
    public partial class MoveToSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Sales");

            migrationBuilder.EnsureSchema(
                name: "Inventory");

            migrationBuilder.EnsureSchema(
                name: "Catalog");

            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "Suppliers",
                newName: "Suppliers",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "Stores",
                newName: "Stores",
                newSchema: "Inventory");

            migrationBuilder.RenameTable(
                name: "StoreInventories",
                newName: "StoreInventories",
                newSchema: "Inventory");

            migrationBuilder.RenameTable(
                name: "StockTransfers",
                newName: "StockTransfers",
                newSchema: "Inventory");

            migrationBuilder.RenameTable(
                name: "StockTransferItems",
                newName: "StockTransferItems",
                newSchema: "Inventory");

            migrationBuilder.RenameTable(
                name: "StockBatches",
                newName: "StockBatches",
                newSchema: "Inventory");

            migrationBuilder.RenameTable(
                name: "Sellers",
                newName: "Sellers",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "SalesSteps",
                newName: "SalesSteps",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "SalesSessions",
                newName: "SalesSessions",
                newSchema: "Sales");

            migrationBuilder.RenameTable(
                name: "PurchaseOrdersItems",
                newName: "PurchaseOrdersItems",
                newSchema: "Sales");

            migrationBuilder.RenameTable(
                name: "PurchaseOrders",
                newName: "PurchaseOrders",
                newSchema: "Sales");

            migrationBuilder.RenameTable(
                name: "ProductSupplier",
                newName: "ProductSupplier",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Products",
                newSchema: "Catalog");

            migrationBuilder.RenameTable(
                name: "InventoryLogs",
                newName: "InventoryLogs",
                newSchema: "Inventory");

            migrationBuilder.RenameTable(
                name: "CartItems",
                newName: "CartItems",
                newSchema: "Sales");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Users",
                schema: "Identity",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Suppliers",
                schema: "Catalog",
                newName: "Suppliers");

            migrationBuilder.RenameTable(
                name: "Stores",
                schema: "Inventory",
                newName: "Stores");

            migrationBuilder.RenameTable(
                name: "StoreInventories",
                schema: "Inventory",
                newName: "StoreInventories");

            migrationBuilder.RenameTable(
                name: "StockTransfers",
                schema: "Inventory",
                newName: "StockTransfers");

            migrationBuilder.RenameTable(
                name: "StockTransferItems",
                schema: "Inventory",
                newName: "StockTransferItems");

            migrationBuilder.RenameTable(
                name: "StockBatches",
                schema: "Inventory",
                newName: "StockBatches");

            migrationBuilder.RenameTable(
                name: "Sellers",
                schema: "Identity",
                newName: "Sellers");

            migrationBuilder.RenameTable(
                name: "SalesSteps",
                schema: "Catalog",
                newName: "SalesSteps");

            migrationBuilder.RenameTable(
                name: "SalesSessions",
                schema: "Sales",
                newName: "SalesSessions");

            migrationBuilder.RenameTable(
                name: "PurchaseOrdersItems",
                schema: "Sales",
                newName: "PurchaseOrdersItems");

            migrationBuilder.RenameTable(
                name: "PurchaseOrders",
                schema: "Sales",
                newName: "PurchaseOrders");

            migrationBuilder.RenameTable(
                name: "ProductSupplier",
                schema: "Catalog",
                newName: "ProductSupplier");

            migrationBuilder.RenameTable(
                name: "Products",
                schema: "Catalog",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "InventoryLogs",
                schema: "Inventory",
                newName: "InventoryLogs");

            migrationBuilder.RenameTable(
                name: "CartItems",
                schema: "Sales",
                newName: "CartItems");
        }
    }
}
