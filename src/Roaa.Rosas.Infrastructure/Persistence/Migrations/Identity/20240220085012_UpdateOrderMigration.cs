using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class UpdateOrderMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Reference",
                table: "rosas_orders",
                newName: "ProcessedPaymentResult");

            migrationBuilder.RenameColumn(
                name: "AuthorizationTransactionResult",
                table: "rosas_orders",
                newName: "ProcessedPaymentReference");

            migrationBuilder.RenameColumn(
                name: "AuthorizationTransactionId",
                table: "rosas_orders",
                newName: "ProcessedPaymentReferenceType");

            migrationBuilder.RenameColumn(
                name: "AuthorizationTransactionCode",
                table: "rosas_orders",
                newName: "ProcessedPaymentId");

            migrationBuilder.AddColumn<string>(
                name: "AltProcessedPaymentId",
                table: "rosas_orders",
                type: "varchar(250)",
                maxLength: 250,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AuthorizedPaymentResult",
                table: "rosas_orders",
                type: "varchar(250)",
                maxLength: 250,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CapturedPaymentResult",
                table: "rosas_orders",
                type: "varchar(250)",
                maxLength: 250,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "TrialPeriodInDays",
                table: "rosas_order_items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltProcessedPaymentId",
                table: "rosas_orders");

            migrationBuilder.DropColumn(
                name: "AuthorizedPaymentResult",
                table: "rosas_orders");

            migrationBuilder.DropColumn(
                name: "CapturedPaymentResult",
                table: "rosas_orders");

            migrationBuilder.DropColumn(
                name: "TrialPeriodInDays",
                table: "rosas_order_items");

            migrationBuilder.RenameColumn(
                name: "ProcessedPaymentResult",
                table: "rosas_orders",
                newName: "Reference");

            migrationBuilder.RenameColumn(
                name: "ProcessedPaymentReferenceType",
                table: "rosas_orders",
                newName: "AuthorizationTransactionId");

            migrationBuilder.RenameColumn(
                name: "ProcessedPaymentReference",
                table: "rosas_orders",
                newName: "AuthorizationTransactionResult");

            migrationBuilder.RenameColumn(
                name: "ProcessedPaymentId",
                table: "rosas_orders",
                newName: "AuthorizationTransactionCode");
        }
    }
}
