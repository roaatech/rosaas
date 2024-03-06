using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class PaymentMethodOfOrderDbMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentMethodType",
                table: "rosas_linked_cards",
                newName: "PaymentPlatform");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "rosas_orders",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "PaymentPlatform",
                table: "rosas_orders",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "rosas_orders");

            migrationBuilder.DropColumn(
                name: "PaymentPlatform",
                table: "rosas_orders");

            migrationBuilder.RenameColumn(
                name: "PaymentPlatform",
                table: "rosas_linked_cards",
                newName: "PaymentMethodType");
        }
    }
}
