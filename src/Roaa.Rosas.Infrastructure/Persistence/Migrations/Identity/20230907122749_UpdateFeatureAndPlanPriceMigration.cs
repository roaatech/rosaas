using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class UpdateFeatureAndPlanPriceMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "rosas_plan_prices",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSubscribed",
                table: "rosas_plan_prices",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSubscribed",
                table: "rosas_features",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "rosas_plan_prices");

            migrationBuilder.DropColumn(
                name: "IsSubscribed",
                table: "rosas_plan_prices");

            migrationBuilder.DropColumn(
                name: "IsSubscribed",
                table: "rosas_features");
        }
    }
}
