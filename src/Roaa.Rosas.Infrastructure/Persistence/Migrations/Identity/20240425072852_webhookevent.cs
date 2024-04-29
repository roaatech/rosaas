using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class webhookevent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "SubscriptionRenewalDate",
                table: "rosas_subscription_renewals",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2034, 4, 25, 7, 28, 51, 841, DateTimeKind.Utc).AddTicks(7979),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2034, 4, 7, 0, 27, 37, 731, DateTimeKind.Utc).AddTicks(3064));

            migrationBuilder.CreateTable(
                name: "rosas_webhook_endpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Url = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SigningSecret = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EntityId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ModifiedByUserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rosas_webhook_endpoints", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RosasWebhookEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    WebhookEndpointId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Event = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RosasWebhookEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RosasWebhookEvents_rosas_webhook_endpoints_WebhookEndpointId",
                        column: x => x.WebhookEndpointId,
                        principalTable: "rosas_webhook_endpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_RosasWebhookEvents_WebhookEndpointId",
                table: "RosasWebhookEvents",
                column: "WebhookEndpointId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RosasWebhookEvents");

            migrationBuilder.DropTable(
                name: "rosas_webhook_endpoints");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SubscriptionRenewalDate",
                table: "rosas_subscription_renewals",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2034, 4, 7, 0, 27, 37, 731, DateTimeKind.Utc).AddTicks(3064),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2034, 4, 25, 7, 28, 51, 841, DateTimeKind.Utc).AddTicks(7979));
        }
    }
}
