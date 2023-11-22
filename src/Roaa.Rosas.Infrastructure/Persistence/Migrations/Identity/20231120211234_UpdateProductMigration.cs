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
                name: "UniqueName",
                table: "rosas_clients",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "rosas_clients",
                newName: "DisplayName");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "rosas_products",
                type: "varchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "rosas_products");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "rosas_clients",
                newName: "UniqueName");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "rosas_clients",
                newName: "Title");
        }
    }
}
