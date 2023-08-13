using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class UpdateProductMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HealthStatusChangeUrl",
                table: "rosas_products",
                newName: "HealthStatusInformerUrl");

            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                table: "rosas_products",
                type: "varchar(250)",
                maxLength: 250,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiKey",
                table: "rosas_products");

            migrationBuilder.RenameColumn(
                name: "HealthStatusInformerUrl",
                table: "rosas_products",
                newName: "HealthStatusChangeUrl");
        }
    }
}
