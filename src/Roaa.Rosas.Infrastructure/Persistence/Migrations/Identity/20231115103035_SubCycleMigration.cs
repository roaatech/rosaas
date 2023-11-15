using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class SubCycleMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_rosas_subscription_feature_cycles_SubscriptionCycleId",
                table: "rosas_subscription_feature_cycles",
                column: "SubscriptionCycleId");

            migrationBuilder.AddForeignKey(
                name: "FK_rosas_subscription_feature_cycles_rosas_subscription_cycles_~",
                table: "rosas_subscription_feature_cycles",
                column: "SubscriptionCycleId",
                principalTable: "rosas_subscription_cycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rosas_subscription_feature_cycles_rosas_subscription_cycles_~",
                table: "rosas_subscription_feature_cycles");

            migrationBuilder.DropIndex(
                name: "IX_rosas_subscription_feature_cycles_SubscriptionCycleId",
                table: "rosas_subscription_feature_cycles");
        }
    }
}
