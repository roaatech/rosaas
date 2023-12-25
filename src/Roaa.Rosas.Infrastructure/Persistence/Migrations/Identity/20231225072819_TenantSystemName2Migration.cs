using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class TenantSystemName2Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rosas_tenant_creation_request_items");

            migrationBuilder.DropColumn(
                name: "SubtotalExclTax",
                table: "rosas_tenant_creation_requests");

            migrationBuilder.DropColumn(
                name: "SubtotalInclTax",
                table: "rosas_tenant_creation_requests");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "rosas_tenant_creation_requests");

            migrationBuilder.DropColumn(
                name: "PurchasedEntityType",
                table: "rosas_order_items");

            migrationBuilder.RenameColumn(
                name: "SystemName",
                table: "rosas_tenant_system_names",
                newName: "TenantNormalizedSystemName");

            migrationBuilder.RenameColumn(
                name: "SystemName",
                table: "rosas_tenant_creation_requests",
                newName: "NormalizedSystemName");

            migrationBuilder.RenameColumn(
                name: "PurchasedEntityId",
                table: "rosas_order_items",
                newName: "PlanPriceId");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantCreationRequestId",
                table: "rosas_tenant_system_names",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "rosas_tenant_creation_requests",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "rosas_orders",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "OrderIntent",
                table: "rosas_orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentProcessingExpirationDate",
                table: "rosas_orders",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomPeriodInDays",
                table: "rosas_order_items",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlanId",
                table: "rosas_order_items",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "rosas_tenant_creation_request_specifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenantCreationRequestId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SpecificationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Value = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rosas_tenant_creation_request_specifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rosas_tenant_creation_request_specifications_rosas_tenant_cr~",
                        column: x => x.TenantCreationRequestId,
                        principalTable: "rosas_tenant_creation_requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_rosas_tenant_creation_request_specifications_TenantCreationR~",
                table: "rosas_tenant_creation_request_specifications",
                column: "TenantCreationRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rosas_tenant_creation_request_specifications");

            migrationBuilder.DropColumn(
                name: "TenantCreationRequestId",
                table: "rosas_tenant_system_names");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "rosas_tenant_creation_requests");

            migrationBuilder.DropColumn(
                name: "OrderIntent",
                table: "rosas_orders");

            migrationBuilder.DropColumn(
                name: "PaymentProcessingExpirationDate",
                table: "rosas_orders");

            migrationBuilder.DropColumn(
                name: "CustomPeriodInDays",
                table: "rosas_order_items");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "rosas_order_items");

            migrationBuilder.RenameColumn(
                name: "TenantNormalizedSystemName",
                table: "rosas_tenant_system_names",
                newName: "SystemName");

            migrationBuilder.RenameColumn(
                name: "NormalizedSystemName",
                table: "rosas_tenant_creation_requests",
                newName: "SystemName");

            migrationBuilder.RenameColumn(
                name: "PlanPriceId",
                table: "rosas_order_items",
                newName: "PurchasedEntityId");

            migrationBuilder.AddColumn<decimal>(
                name: "SubtotalExclTax",
                table: "rosas_tenant_creation_requests",
                type: "decimal(8,2)",
                precision: 8,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubtotalInclTax",
                table: "rosas_tenant_creation_requests",
                type: "decimal(8,2)",
                precision: 8,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "rosas_tenant_creation_requests",
                type: "decimal(8,2)",
                precision: 8,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "rosas_orders",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "PurchasedEntityType",
                table: "rosas_order_items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "rosas_tenant_creation_request_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenantCreationRequestId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ClientId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CustomPeriodInDays = table.Column<int>(type: "int", nullable: true),
                    DisplayName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    PlanId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanPriceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PriceExclTax = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    PriceInclTax = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Specifications = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    SystemName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UnitPriceExclTax = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    UnitPriceInclTax = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rosas_tenant_creation_request_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rosas_tenant_creation_request_items_rosas_tenant_creation_re~",
                        column: x => x.TenantCreationRequestId,
                        principalTable: "rosas_tenant_creation_requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_rosas_tenant_creation_request_items_TenantCreationRequestId",
                table: "rosas_tenant_creation_request_items",
                column: "TenantCreationRequestId");
        }
    }
}
