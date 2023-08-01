using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class ProductTenantHealthStatusMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Created",
                table: "rosas_tenant_health_checks",
                newName: "TimeStamp");

            migrationBuilder.CreateTable(
                name: "rosas_product_tenant_health_statuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsHealthy = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HealthCheckUrl = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastCheckDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CheckDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rosas_product_tenant_health_statuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rosas_product_tenant_health_statuses_rosas_product_tenants_Id",
                        column: x => x.Id,
                        principalTable: "rosas_product_tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rosas_product_tenant_health_statuses");

            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "rosas_tenant_health_checks",
                newName: "Created");
        }
    }
}
