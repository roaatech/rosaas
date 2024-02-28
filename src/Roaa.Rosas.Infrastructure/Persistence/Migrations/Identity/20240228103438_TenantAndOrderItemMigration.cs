using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class TenantAndOrderItemMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserType",
                table: "rosas_tenants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "SubscriptionId",
                table: "rosas_order_items",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_rosas_order_items_SubscriptionId",
                table: "rosas_order_items",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_rosas_order_items_rosas_subscriptions_SubscriptionId",
                table: "rosas_order_items",
                column: "SubscriptionId",
                principalTable: "rosas_subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rosas_order_items_rosas_subscriptions_SubscriptionId",
                table: "rosas_order_items");

            migrationBuilder.DropIndex(
                name: "IX_rosas_order_items_SubscriptionId",
                table: "rosas_order_items");

            migrationBuilder.DropColumn(
                name: "CreatedByUserType",
                table: "rosas_tenants");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubscriptionId",
                table: "rosas_order_items",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");
        }
    }
}
