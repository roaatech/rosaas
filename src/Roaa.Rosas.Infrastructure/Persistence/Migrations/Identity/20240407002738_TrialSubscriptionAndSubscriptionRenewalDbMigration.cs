using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class TrialSubscriptionAndSubscriptionRenewalDbMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rosas_subscription_auto_renewal_histories");

            migrationBuilder.DropTable(
                name: "rosas_subscription_auto_renewals");

            migrationBuilder.DropTable(
                name: "rosas_subscription_plan_change_histories");

            migrationBuilder.DropTable(
                name: "rosas_subscription_plan_changes");

            migrationBuilder.DropTable(
                name: "rosas_subscription_trial_periods");

            migrationBuilder.DropColumn(
                name: "SubscriptionPlanChangeStatus",
                table: "rosas_subscriptions");

            migrationBuilder.RenameColumn(
                name: "OrderIntent",
                table: "rosas_orders",
                newName: "OrderType");

            migrationBuilder.AddColumn<bool>(
                name: "ApplySubscriptionDowngradeByExternalSystemAction",
                table: "rosas_products",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApplySubscriptionUpgradeByExternalSystemAction",
                table: "rosas_products",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "rosas_subscription_renewal_histories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanPriceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SubscriptionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanCycle = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    Comment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RenewalDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    RenewalEnabledDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    RenewalEnabledByUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rosas_subscription_renewal_histories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rosas_subscription_renewals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanPriceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SubscriptionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanCycle = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    PlanDisplayName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubscriptionRenewalDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValue: new DateTime(2034, 4, 7, 0, 27, 37, 731, DateTimeKind.Utc).AddTicks(3064)),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RenewalsCount = table.Column<int>(type: "int", nullable: false),
                    IsContinuousRenewal = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsForced = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Comment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedByUserType = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ModifiedByUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rosas_subscription_renewals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rosas_subscription_renewals_rosas_plan_prices_PlanPriceId",
                        column: x => x.PlanPriceId,
                        principalTable: "rosas_plan_prices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_rosas_subscription_renewals_rosas_plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "rosas_plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_rosas_subscription_renewals_rosas_subscriptions_Id",
                        column: x => x.Id,
                        principalTable: "rosas_subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rosas_trial_subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SubscriptionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TrialPlanId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TrialPlanPriceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SelectedPlanId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SelectedPlanPriceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    TrialPeriodInDays = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rosas_trial_subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rosas_trial_subscriptions_rosas_subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "rosas_subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_rosas_subscription_renewals_PlanId",
                table: "rosas_subscription_renewals",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_rosas_subscription_renewals_PlanPriceId",
                table: "rosas_subscription_renewals",
                column: "PlanPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_rosas_trial_subscriptions_SubscriptionId",
                table: "rosas_trial_subscriptions",
                column: "SubscriptionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rosas_subscription_renewal_histories");

            migrationBuilder.DropTable(
                name: "rosas_subscription_renewals");

            migrationBuilder.DropTable(
                name: "rosas_trial_subscriptions");

            migrationBuilder.DropColumn(
                name: "ApplySubscriptionDowngradeByExternalSystemAction",
                table: "rosas_products");

            migrationBuilder.DropColumn(
                name: "ApplySubscriptionUpgradeByExternalSystemAction",
                table: "rosas_products");

            migrationBuilder.RenameColumn(
                name: "OrderType",
                table: "rosas_orders",
                newName: "OrderIntent");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionPlanChangeStatus",
                table: "rosas_subscriptions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "rosas_subscription_auto_renewal_histories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AutoRenewalEnabledByUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AutoRenewalEnabledDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Comment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PlanCycle = table.Column<int>(type: "int", nullable: false),
                    PlanId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanPriceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Price = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    RenewalDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rosas_subscription_auto_renewal_histories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rosas_subscription_auto_renewals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanPriceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Comment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedByUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsPaid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanCycle = table.Column<int>(type: "int", nullable: false),
                    PlanDisplayName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UpcomingAutoRenewalsCount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rosas_subscription_auto_renewals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rosas_subscription_auto_renewals_rosas_plan_prices_PlanPrice~",
                        column: x => x.PlanPriceId,
                        principalTable: "rosas_plan_prices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_rosas_subscription_auto_renewals_rosas_plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "rosas_plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_rosas_subscription_auto_renewals_rosas_subscriptions_Id",
                        column: x => x.Id,
                        principalTable: "rosas_subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rosas_subscription_plan_change_histories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ChangeDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Comment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PlanChangeEnabledByUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanChangeEnabledDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    PlanCycle = table.Column<int>(type: "int", nullable: false),
                    PlanId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanPriceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Price = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rosas_subscription_plan_change_histories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rosas_subscription_plan_changes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanPriceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SubscriptionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Comment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedByUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsPaid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanCycle = table.Column<int>(type: "int", nullable: false),
                    PlanDisplayName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rosas_subscription_plan_changes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rosas_subscription_plan_changes_rosas_plan_prices_PlanPriceId",
                        column: x => x.PlanPriceId,
                        principalTable: "rosas_plan_prices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_rosas_subscription_plan_changes_rosas_plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "rosas_plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_rosas_subscription_plan_changes_rosas_subscriptions_Subscrip~",
                        column: x => x.SubscriptionId,
                        principalTable: "rosas_subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rosas_subscription_trial_periods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SubscriptionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    SelectedPlanId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SelectedPlanPriceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    TrialPeriodInDays = table.Column<int>(type: "int", nullable: false),
                    TrialPlanId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TrialPlanPriceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rosas_subscription_trial_periods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rosas_subscription_trial_periods_rosas_subscriptions_Subscri~",
                        column: x => x.SubscriptionId,
                        principalTable: "rosas_subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_rosas_subscription_auto_renewals_PlanId",
                table: "rosas_subscription_auto_renewals",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_rosas_subscription_auto_renewals_PlanPriceId",
                table: "rosas_subscription_auto_renewals",
                column: "PlanPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_rosas_subscription_plan_changes_PlanId",
                table: "rosas_subscription_plan_changes",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_rosas_subscription_plan_changes_PlanPriceId",
                table: "rosas_subscription_plan_changes",
                column: "PlanPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_rosas_subscription_plan_changes_SubscriptionId",
                table: "rosas_subscription_plan_changes",
                column: "SubscriptionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rosas_subscription_trial_periods_SubscriptionId",
                table: "rosas_subscription_trial_periods",
                column: "SubscriptionId",
                unique: true);
        }
    }
}
