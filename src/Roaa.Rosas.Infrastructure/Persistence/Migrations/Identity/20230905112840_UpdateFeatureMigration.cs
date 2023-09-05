using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class UpdateFeatureMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "rosas_features");

            migrationBuilder.AddColumn<int>(
                name: "Unit",
                table: "rosas_plan_features",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "rosas_plan_features");

            migrationBuilder.AddColumn<int>(
                name: "Unit",
                table: "rosas_features",
                type: "int",
                nullable: true);
        }
    }
}
