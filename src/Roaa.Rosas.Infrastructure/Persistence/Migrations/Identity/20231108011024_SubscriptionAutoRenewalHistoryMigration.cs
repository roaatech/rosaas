using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class SubscriptionAutoRenewalHistoryMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "rosas_tenants",
                newName: "DisplayName");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "rosas_tenant_status_history",
                newName: "CreationDate");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "rosas_tenant_health_check_history",
                newName: "CreationDate");

            migrationBuilder.RenameColumn(
                name: "FeatureName",
                table: "rosas_subscription_feature_cycles",
                newName: "FeatureDisplayName");

            migrationBuilder.RenameColumn(
                name: "PlanName",
                table: "rosas_subscription_cycles",
                newName: "PlanDisplayName");

            migrationBuilder.RenameColumn(
                name: "Cycle",
                table: "rosas_subscription_auto_renewals",
                newName: "PlanCycle");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "rosas_plans",
                newName: "DisplayName");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "rosas_job_tasks",
                newName: "CreationDate");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "rosas_features",
                newName: "DisplayName");

            migrationBuilder.AddColumn<string>(
                name: "PlanDisplayName",
                table: "rosas_subscription_auto_renewals",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rosas_subscription_auto_renewal_histories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlanPriceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SubscriptionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Cycle = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    Comment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RenewalDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    AutoRenewalEnabledDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    AutoRenewalEnabledByUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rosas_subscription_auto_renewal_histories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rosas_subscription_auto_renewal_histories");

            migrationBuilder.DropColumn(
                name: "PlanDisplayName",
                table: "rosas_subscription_auto_renewals");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "rosas_tenants",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "rosas_tenant_status_history",
                newName: "Created");

            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "rosas_tenant_health_check_history",
                newName: "Created");

            migrationBuilder.RenameColumn(
                name: "FeatureDisplayName",
                table: "rosas_subscription_feature_cycles",
                newName: "FeatureName");

            migrationBuilder.RenameColumn(
                name: "PlanDisplayName",
                table: "rosas_subscription_cycles",
                newName: "PlanName");

            migrationBuilder.RenameColumn(
                name: "PlanCycle",
                table: "rosas_subscription_auto_renewals",
                newName: "Cycle");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "rosas_plans",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "rosas_job_tasks",
                newName: "Created");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "rosas_features",
                newName: "Title");
        }
    }
}
