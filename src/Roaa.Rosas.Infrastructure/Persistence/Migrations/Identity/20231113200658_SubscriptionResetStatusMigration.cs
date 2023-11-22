using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class SubscriptionResetStatusMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ResetOperationDate",
                table: "rosas_subscriptions",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionResetStatus",
                table: "rosas_subscriptions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetOperationDate",
                table: "rosas_subscriptions");

            migrationBuilder.DropColumn(
                name: "SubscriptionResetStatus",
                table: "rosas_subscriptions");
        }
    }
}
