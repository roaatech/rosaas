using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class AddProductTenantEntityDbMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mng_Tenants_mng_Products_ProductId",
                table: "mng_Tenants");

            migrationBuilder.DropIndex(
                name: "IX_mng_Tenants_ProductId",
                table: "mng_Tenants");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "mng_Tenants");

            migrationBuilder.CreateTable(
                name: "mng_ProductTenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TenantId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mng_ProductTenants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mng_ProductTenants_mng_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "mng_Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_mng_ProductTenants_mng_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "mng_Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_mng_ProductTenants_ProductId",
                table: "mng_ProductTenants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_mng_ProductTenants_TenantId",
                table: "mng_ProductTenants",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mng_ProductTenants");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "mng_Tenants",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_mng_Tenants_ProductId",
                table: "mng_Tenants",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_mng_Tenants_mng_Products_ProductId",
                table: "mng_Tenants",
                column: "ProductId",
                principalTable: "mng_Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
