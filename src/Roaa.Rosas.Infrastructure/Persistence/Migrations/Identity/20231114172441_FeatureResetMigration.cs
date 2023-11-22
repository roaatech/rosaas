using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class FeatureResetMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rosas_subscription_feature_cycles_rosas_subscription_feature~",
                table: "rosas_subscription_feature_cycles");

            migrationBuilder.DropIndex(
                name: "IX_rosas_subscription_feature_cycles_SubscriptionFeatureId",
                table: "rosas_subscription_feature_cycles");

            migrationBuilder.RenameColumn(
                name: "Reset",
                table: "rosas_features",
                newName: "FeatureReset");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FeatureReset",
                table: "rosas_features",
                newName: "Reset");

            migrationBuilder.CreateIndex(
                name: "IX_rosas_subscription_feature_cycles_SubscriptionFeatureId",
                table: "rosas_subscription_feature_cycles",
                column: "SubscriptionFeatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_rosas_subscription_feature_cycles_rosas_subscription_feature~",
                table: "rosas_subscription_feature_cycles",
                column: "SubscriptionFeatureId",
                principalTable: "rosas_subscription_features",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
