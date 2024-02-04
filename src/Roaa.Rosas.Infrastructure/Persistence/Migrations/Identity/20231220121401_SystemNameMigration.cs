using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class SystemNameMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rosas_tenant_names");

            migrationBuilder.RenameColumn(
                name: "UniqueName",
                table: "rosas_tenants",
                newName: "SystemName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "rosas_tenant_creation_requests",
                newName: "SystemName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "rosas_tenant_creation_request_items",
                newName: "SystemName");

            migrationBuilder.RenameColumn(
                name: "NormalizedName",
                table: "rosas_specifications",
                newName: "SystemName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "rosas_specifications",
                newName: "NormalizedSystemName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "rosas_products",
                newName: "SystemName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "rosas_plans",
                newName: "SystemName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "rosas_plan_prices",
                newName: "SystemName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "rosas_order_items",
                newName: "SystemName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "rosas_features",
                newName: "SystemName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "rosas_clients",
                newName: "SystemName");

            migrationBuilder.CreateTable(
                name: "rosas_tenant_system_names",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SystemName = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rosas_tenant_system_names", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rosas_tenant_system_names");

            migrationBuilder.RenameColumn(
                name: "SystemName",
                table: "rosas_tenants",
                newName: "UniqueName");

            migrationBuilder.RenameColumn(
                name: "SystemName",
                table: "rosas_tenant_creation_requests",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SystemName",
                table: "rosas_tenant_creation_request_items",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SystemName",
                table: "rosas_specifications",
                newName: "NormalizedName");

            migrationBuilder.RenameColumn(
                name: "NormalizedSystemName",
                table: "rosas_specifications",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SystemName",
                table: "rosas_products",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SystemName",
                table: "rosas_plans",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SystemName",
                table: "rosas_plan_prices",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SystemName",
                table: "rosas_order_items",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SystemName",
                table: "rosas_features",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SystemName",
                table: "rosas_clients",
                newName: "Name");

            migrationBuilder.CreateTable(
                name: "rosas_tenant_names",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rosas_tenant_names", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
