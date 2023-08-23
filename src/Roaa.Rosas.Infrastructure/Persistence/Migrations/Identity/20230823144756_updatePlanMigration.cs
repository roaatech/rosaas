using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class updatePlanMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "rosas_plans",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_rosas_plans_ProductId",
                table: "rosas_plans",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_rosas_plans_rosas_products_ProductId",
                table: "rosas_plans",
                column: "ProductId",
                principalTable: "rosas_products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rosas_plans_rosas_products_ProductId",
                table: "rosas_plans");

            migrationBuilder.DropIndex(
                name: "IX_rosas_plans_ProductId",
                table: "rosas_plans");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "rosas_plans");
        }
    }
}
