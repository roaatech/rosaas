using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class TenantStatusDbMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "rosas_tenants");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "rosas_tenant_processes",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<DateTime>(
                name: "Edited",
                table: "rosas_product_tenants",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "EditedByUserId",
                table: "rosas_product_tenants",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "rosas_product_tenants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "rosas_tenant_processes");

            migrationBuilder.DropColumn(
                name: "Edited",
                table: "rosas_product_tenants");

            migrationBuilder.DropColumn(
                name: "EditedByUserId",
                table: "rosas_product_tenants");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "rosas_product_tenants");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "rosas_tenants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
