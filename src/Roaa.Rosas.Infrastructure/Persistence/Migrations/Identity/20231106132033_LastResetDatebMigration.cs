using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class LastResetDatebMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastLimitsResetDate",
                table: "rosas_subscriptions",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastResetDate",
                table: "rosas_subscriptions",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResetUrl",
                table: "rosas_products",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLimitsResetDate",
                table: "rosas_subscriptions");

            migrationBuilder.DropColumn(
                name: "LastResetDate",
                table: "rosas_subscriptions");

            migrationBuilder.DropColumn(
                name: "ResetUrl",
                table: "rosas_products");
        }
    }
}
