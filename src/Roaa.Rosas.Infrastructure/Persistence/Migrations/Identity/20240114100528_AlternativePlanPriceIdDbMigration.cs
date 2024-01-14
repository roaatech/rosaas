using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class AlternativePlanPriceIdDbMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AlternativePlanID",
                table: "rosas_plans",
                newName: "AlternativePlanId");

            migrationBuilder.AddColumn<Guid>(
                name: "AlternativePlanPriceId",
                table: "rosas_plans",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlternativePlanPriceId",
                table: "rosas_plans");

            migrationBuilder.RenameColumn(
                name: "AlternativePlanId",
                table: "rosas_plans",
                newName: "AlternativePlanID");
        }
    }
}
