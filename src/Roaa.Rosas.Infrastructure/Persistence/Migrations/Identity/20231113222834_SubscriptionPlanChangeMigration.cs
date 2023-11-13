using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class SubscriptionPlanChangeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubscriptionPlanChangeStatus",
                table: "rosas_subscriptions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionDowngradeUrl",
                table: "rosas_products",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionUpgradeUrl",
                table: "rosas_products",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscriptionPlanChangeStatus",
                table: "rosas_subscriptions");

            migrationBuilder.DropColumn(
                name: "SubscriptionDowngradeUrl",
                table: "rosas_products");

            migrationBuilder.DropColumn(
                name: "SubscriptionUpgradeUrl",
                table: "rosas_products");
        }
    }
}
