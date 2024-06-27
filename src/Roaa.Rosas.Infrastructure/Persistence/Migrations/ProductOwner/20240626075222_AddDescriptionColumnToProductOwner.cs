using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.ProductOwner
{
    /// <inheritdoc />
    public partial class AddDescriptionColumnToProductOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "SubscriptionRenewalDate",
                table: "rosas_subscription_renewals",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2034, 4, 25, 7, 28, 51, 841, DateTimeKind.Utc).AddTicks(7979));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "rosas_clients",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "rosas_clients");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SubscriptionRenewalDate",
                table: "rosas_subscription_renewals",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2034, 4, 25, 7, 28, 51, 841, DateTimeKind.Utc).AddTicks(7979),
                oldClrType: typeof(DateTime),
                oldType: "datetime");
        }
    }
}
