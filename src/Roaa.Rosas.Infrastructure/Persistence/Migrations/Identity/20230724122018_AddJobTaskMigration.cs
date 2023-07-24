using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class AddJobTaskMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "rosas_products",
                newName: "HealthStatusChangeUrl");

            migrationBuilder.RenameColumn(
                name: "UniqueName",
                table: "rosas_products",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "rosas_products",
                newName: "DeletionUrl");

            migrationBuilder.RenameColumn(
                name: "DeletionEndpoint",
                table: "rosas_products",
                newName: "DefaultHealthCheckUrl");

            migrationBuilder.RenameColumn(
                name: "DeactivationEndpoint",
                table: "rosas_products",
                newName: "DeactivationUrl");

            migrationBuilder.RenameColumn(
                name: "CreationEndpoint",
                table: "rosas_products",
                newName: "CreationUrl");

            migrationBuilder.RenameColumn(
                name: "ActivationEndpoint",
                table: "rosas_products",
                newName: "ActivationUrl");

            migrationBuilder.AddColumn<string>(
                name: "HealthCheckUrl",
                table: "rosas_product_tenants",
                type: "varchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "HealthCheckUrlIsOverridden",
                table: "rosas_product_tenants",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "rosas_job_tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rosas_job_tasks", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rosas_tenant_health_checks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    IsHealthy = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    HealthCheckUrl = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rosas_tenant_health_checks", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rosas_job_tasks");

            migrationBuilder.DropTable(
                name: "rosas_tenant_health_checks");

            migrationBuilder.DropColumn(
                name: "HealthCheckUrl",
                table: "rosas_product_tenants");

            migrationBuilder.DropColumn(
                name: "HealthCheckUrlIsOverridden",
                table: "rosas_product_tenants");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "rosas_products",
                newName: "UniqueName");

            migrationBuilder.RenameColumn(
                name: "HealthStatusChangeUrl",
                table: "rosas_products",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "DeletionUrl",
                table: "rosas_products",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "DefaultHealthCheckUrl",
                table: "rosas_products",
                newName: "DeletionEndpoint");

            migrationBuilder.RenameColumn(
                name: "DeactivationUrl",
                table: "rosas_products",
                newName: "DeactivationEndpoint");

            migrationBuilder.RenameColumn(
                name: "CreationUrl",
                table: "rosas_products",
                newName: "CreationEndpoint");

            migrationBuilder.RenameColumn(
                name: "ActivationUrl",
                table: "rosas_products",
                newName: "ActivationEndpoint");
        }
    }
}
