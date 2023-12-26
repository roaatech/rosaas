using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class ProductTrialTypeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TrialPeriodInDays",
                table: "rosas_products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TrialPlanId",
                table: "rosas_products",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "TrialType",
                table: "rosas_products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "AlternativePlanID",
                table: "rosas_plans",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "TrialPeriodInDays",
                table: "rosas_plans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrialPeriodInDays",
                table: "rosas_products");

            migrationBuilder.DropColumn(
                name: "TrialPlanId",
                table: "rosas_products");

            migrationBuilder.DropColumn(
                name: "TrialType",
                table: "rosas_products");

            migrationBuilder.DropColumn(
                name: "AlternativePlanID",
                table: "rosas_plans");

            migrationBuilder.DropColumn(
                name: "TrialPeriodInDays",
                table: "rosas_plans");
        }
    }
}
