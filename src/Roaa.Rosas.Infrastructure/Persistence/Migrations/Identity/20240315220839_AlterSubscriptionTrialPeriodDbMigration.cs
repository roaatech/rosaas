using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class AlterSubscriptionTrialPeriodDbMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlanPriceId",
                table: "rosas_subscription_trial_periods",
                newName: "TrialPlanPriceId");

            migrationBuilder.RenameColumn(
                name: "PlanId",
                table: "rosas_subscription_trial_periods",
                newName: "TrialPlanId");

            migrationBuilder.AddColumn<int>(
                name: "SequenceNum",
                table: "rosas_order_items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SequenceNum",
                table: "rosas_order_items");

            migrationBuilder.RenameColumn(
                name: "TrialPlanPriceId",
                table: "rosas_subscription_trial_periods",
                newName: "PlanPriceId");

            migrationBuilder.RenameColumn(
                name: "TrialPlanId",
                table: "rosas_subscription_trial_periods",
                newName: "PlanId");
        }
    }
}
