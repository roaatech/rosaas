using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class RenamePropertiesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Unit",
                table: "rosas_subscription_feature_cycles",
                newName: "FeatureUnit");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "rosas_subscription_feature_cycles",
                newName: "PlanCycle");

            migrationBuilder.RenameColumn(
                name: "Reset",
                table: "rosas_subscription_feature_cycles",
                newName: "FeatureType");

            migrationBuilder.RenameColumn(
                name: "Cycle",
                table: "rosas_subscription_feature_cycles",
                newName: "FeatureReset");

            migrationBuilder.RenameColumn(
                name: "Cycle",
                table: "rosas_plan_prices",
                newName: "PlanCycle");

            migrationBuilder.RenameColumn(
                name: "Unit",
                table: "rosas_plan_features",
                newName: "FeatureUnit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlanCycle",
                table: "rosas_subscription_feature_cycles",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "FeatureUnit",
                table: "rosas_subscription_feature_cycles",
                newName: "Unit");

            migrationBuilder.RenameColumn(
                name: "FeatureType",
                table: "rosas_subscription_feature_cycles",
                newName: "Reset");

            migrationBuilder.RenameColumn(
                name: "FeatureReset",
                table: "rosas_subscription_feature_cycles",
                newName: "Cycle");

            migrationBuilder.RenameColumn(
                name: "PlanCycle",
                table: "rosas_plan_prices",
                newName: "Cycle");

            migrationBuilder.RenameColumn(
                name: "FeatureUnit",
                table: "rosas_plan_features",
                newName: "Unit");
        }
    }
}
