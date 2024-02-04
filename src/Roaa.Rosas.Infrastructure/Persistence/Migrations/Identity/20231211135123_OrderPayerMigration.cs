using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class OrderPayerMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserType",
                table: "rosas_orders",
                newName: "PayerUserType");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserType",
                table: "rosas_orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "PayerUserId",
                table: "rosas_orders",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserType",
                table: "rosas_orders");

            migrationBuilder.DropColumn(
                name: "PayerUserId",
                table: "rosas_orders");

            migrationBuilder.RenameColumn(
                name: "PayerUserType",
                table: "rosas_orders",
                newName: "UserType");
        }
    }
}
