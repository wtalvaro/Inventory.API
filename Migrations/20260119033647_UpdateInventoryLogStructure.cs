using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInventoryLogStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "InventoryLogs",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "InventoryLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "InventoryLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuantityChange",
                table: "InventoryLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "InventoryLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "InventoryLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLogs_ProductId",
                table: "InventoryLogs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLogs_StoreId",
                table: "InventoryLogs",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryLogs_Products_ProductId",
                table: "InventoryLogs",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryLogs_Stores_StoreId",
                table: "InventoryLogs",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryLogs_Products_ProductId",
                table: "InventoryLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryLogs_Stores_StoreId",
                table: "InventoryLogs");

            migrationBuilder.DropIndex(
                name: "IX_InventoryLogs_ProductId",
                table: "InventoryLogs");

            migrationBuilder.DropIndex(
                name: "IX_InventoryLogs_StoreId",
                table: "InventoryLogs");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "InventoryLogs");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "InventoryLogs");

            migrationBuilder.DropColumn(
                name: "QuantityChange",
                table: "InventoryLogs");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "InventoryLogs");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "InventoryLogs");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "InventoryLogs",
                newName: "Timestamp");
        }
    }
}
