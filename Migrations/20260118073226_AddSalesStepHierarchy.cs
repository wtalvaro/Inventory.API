using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesStepHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SalesSteps_Category",
                table: "SalesSteps",
                column: "Category",
                filter: "\"Category\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SalesSteps_IsGlobal",
                table: "SalesSteps",
                column: "IsGlobal",
                filter: "\"IsGlobal\" = TRUE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SalesSteps_Category",
                table: "SalesSteps");

            migrationBuilder.DropIndex(
                name: "IX_SalesSteps_IsGlobal",
                table: "SalesSteps");
        }
    }
}
