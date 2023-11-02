using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class SupportStepMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PreviousStep",
                table: "rosas_tenant_status_history",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Step",
                table: "rosas_tenant_status_history",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Step",
                table: "rosas_tenant_process_history",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Step",
                table: "rosas_subscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviousStep",
                table: "rosas_tenant_status_history");

            migrationBuilder.DropColumn(
                name: "Step",
                table: "rosas_tenant_status_history");

            migrationBuilder.DropColumn(
                name: "Step",
                table: "rosas_tenant_process_history");

            migrationBuilder.DropColumn(
                name: "Step",
                table: "rosas_subscriptions");
        }
    }
}
