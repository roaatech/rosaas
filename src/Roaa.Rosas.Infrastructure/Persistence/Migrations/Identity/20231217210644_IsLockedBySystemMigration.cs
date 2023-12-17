using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class IsLockedBySystemMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLockedBySystem",
                table: "rosas_plans",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TenancyType",
                table: "rosas_plans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsLockedBySystem",
                table: "rosas_plan_prices",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLockedBySystem",
                table: "rosas_entity_admin_privileges",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLockedBySystem",
                table: "identity_users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLockedBySystem",
                table: "rosas_plans");

            migrationBuilder.DropColumn(
                name: "TenancyType",
                table: "rosas_plans");

            migrationBuilder.DropColumn(
                name: "IsLockedBySystem",
                table: "rosas_plan_prices");

            migrationBuilder.DropColumn(
                name: "IsLockedBySystem",
                table: "rosas_entity_admin_privileges");

            migrationBuilder.DropColumn(
                name: "IsLockedBySystem",
                table: "identity_users");
        }
    }
}
