﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roaa.Rosas.Infrastructure.Persistence.Migrations.IdentityServerConfiguration
{
    /// <inheritdoc />
    public partial class InitIdentityServerConfigurationDbMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_api_resources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AllowedAccessTokenSigningAlgorithms = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShowInDiscoveryDocument = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastAccessed = table.Column<DateTime>(type: "datetime", nullable: true),
                    NonEditable = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_api_resources", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_api_scopes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Required = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Emphasize = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ShowInDiscoveryDocument = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_api_scopes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ClientId = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProtocolType = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RequireClientSecret = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ClientName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientUri = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LogoUri = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RequireConsent = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AllowRememberConsent = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AlwaysIncludeUserClaimsInIdToken = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RequirePkce = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AllowPlainTextPkce = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RequireRequestObject = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AllowAccessTokensViaBrowser = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FrontChannelLogoutUri = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FrontChannelLogoutSessionRequired = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BackChannelLogoutUri = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BackChannelLogoutSessionRequired = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AllowOfflineAccess = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdentityTokenLifetime = table.Column<int>(type: "int", nullable: false),
                    AllowedIdentityTokenSigningAlgorithms = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccessTokenLifetime = table.Column<int>(type: "int", nullable: false),
                    AuthorizationCodeLifetime = table.Column<int>(type: "int", nullable: false),
                    ConsentLifetime = table.Column<int>(type: "int", nullable: true),
                    AbsoluteRefreshTokenLifetime = table.Column<int>(type: "int", nullable: false),
                    SlidingRefreshTokenLifetime = table.Column<int>(type: "int", nullable: false),
                    RefreshTokenUsage = table.Column<int>(type: "int", nullable: false),
                    UpdateAccessTokenClaimsOnRefresh = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RefreshTokenExpiration = table.Column<int>(type: "int", nullable: false),
                    AccessTokenType = table.Column<int>(type: "int", nullable: false),
                    EnableLocalLogin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IncludeJwtId = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AlwaysSendClientClaims = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ClientClaimsPrefix = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PairWiseSubjectSalt = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastAccessed = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserSsoLifetime = table.Column<int>(type: "int", nullable: true),
                    UserCodeType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeviceCodeLifetime = table.Column<int>(type: "int", nullable: false),
                    NonEditable = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_clients", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_identity_resources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Required = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Emphasize = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ShowInDiscoveryDocument = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    NonEditable = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_identity_resources", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_api_resource_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ApiResourceId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_api_resource_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ids4_api_resource_claims_ids4_api_resources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "ids4_api_resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_api_resource_properties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ApiResourceId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_api_resource_properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ids4_api_resource_properties_ids4_api_resources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "ids4_api_resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_api_resource_scopes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Scope = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ApiResourceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_api_resource_scopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ids4_api_resource_scopes_ids4_api_resources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "ids4_api_resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_api_resource_secrets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ApiResourceId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Expiration = table.Column<DateTime>(type: "datetime", nullable: true),
                    Type = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_api_resource_secrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ids4_api_resource_secrets_ids4_api_resources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "ids4_api_resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_api_scope_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScopeId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_api_scope_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ids4_api_scope_claims_ids4_api_scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "ids4_api_scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_api_scope_properties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScopeId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_api_scope_properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ids4_api_scope_properties_ids4_api_scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "ids4_api_scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_client_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_client_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ids4_client_claims_ids4_clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ids4_clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_client_cors_origins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Origin = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_client_cors_origins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ids4_client_cors_origins_ids4_clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ids4_clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_client_grant_types",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GrantType = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_client_grant_types", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ids4_client_grant_types_ids4_clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ids4_clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_client_id_p_restrictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Provider = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_client_id_p_restrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ids4_client_id_p_restrictions_ids4_clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ids4_clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_client_post_logout_redirect_uris",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PostLogoutRedirectUri = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_client_post_logout_redirect_uris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ids4_client_post_logout_redirect_uris_ids4_clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ids4_clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_client_properties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_client_properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ids4_client_properties_ids4_clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ids4_clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_client_redirect_uris",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RedirectUri = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_client_redirect_uris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ids4_client_redirect_uris_ids4_clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ids4_clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_client_scopes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Scope = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_client_scopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ids4_client_scopes_ids4_clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ids4_clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_client_secrets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Expiration = table.Column<DateTime>(type: "datetime", nullable: true),
                    Type = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_client_secrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ids4_client_secrets_ids4_clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ids4_clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_identity_resource_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdentityResourceId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_identity_resource_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ids4_identity_resource_claims_ids4_identity_resources_Identi~",
                        column: x => x.IdentityResourceId,
                        principalTable: "ids4_identity_resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ids4_identity_resource_properties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdentityResourceId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ids4_identity_resource_properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ids4_identity_resource_properties_ids4_identity_resources_Id~",
                        column: x => x.IdentityResourceId,
                        principalTable: "ids4_identity_resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_api_resource_claims_ApiResourceId",
                table: "ids4_api_resource_claims",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_api_resource_properties_ApiResourceId",
                table: "ids4_api_resource_properties",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_api_resource_scopes_ApiResourceId",
                table: "ids4_api_resource_scopes",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_api_resource_secrets_ApiResourceId",
                table: "ids4_api_resource_secrets",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_api_resources_Name",
                table: "ids4_api_resources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ids4_api_scope_claims_ScopeId",
                table: "ids4_api_scope_claims",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_api_scope_properties_ScopeId",
                table: "ids4_api_scope_properties",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_api_scopes_Name",
                table: "ids4_api_scopes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ids4_client_claims_ClientId",
                table: "ids4_client_claims",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_client_cors_origins_ClientId",
                table: "ids4_client_cors_origins",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_client_grant_types_ClientId",
                table: "ids4_client_grant_types",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_client_id_p_restrictions_ClientId",
                table: "ids4_client_id_p_restrictions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_client_post_logout_redirect_uris_ClientId",
                table: "ids4_client_post_logout_redirect_uris",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_client_properties_ClientId",
                table: "ids4_client_properties",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_client_redirect_uris_ClientId",
                table: "ids4_client_redirect_uris",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_client_scopes_ClientId",
                table: "ids4_client_scopes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_client_secrets_ClientId",
                table: "ids4_client_secrets",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_clients_ClientId",
                table: "ids4_clients",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ids4_identity_resource_claims_IdentityResourceId",
                table: "ids4_identity_resource_claims",
                column: "IdentityResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_identity_resource_properties_IdentityResourceId",
                table: "ids4_identity_resource_properties",
                column: "IdentityResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ids4_identity_resources_Name",
                table: "ids4_identity_resources",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ids4_api_resource_claims");

            migrationBuilder.DropTable(
                name: "ids4_api_resource_properties");

            migrationBuilder.DropTable(
                name: "ids4_api_resource_scopes");

            migrationBuilder.DropTable(
                name: "ids4_api_resource_secrets");

            migrationBuilder.DropTable(
                name: "ids4_api_scope_claims");

            migrationBuilder.DropTable(
                name: "ids4_api_scope_properties");

            migrationBuilder.DropTable(
                name: "ids4_client_claims");

            migrationBuilder.DropTable(
                name: "ids4_client_cors_origins");

            migrationBuilder.DropTable(
                name: "ids4_client_grant_types");

            migrationBuilder.DropTable(
                name: "ids4_client_id_p_restrictions");

            migrationBuilder.DropTable(
                name: "ids4_client_post_logout_redirect_uris");

            migrationBuilder.DropTable(
                name: "ids4_client_properties");

            migrationBuilder.DropTable(
                name: "ids4_client_redirect_uris");

            migrationBuilder.DropTable(
                name: "ids4_client_scopes");

            migrationBuilder.DropTable(
                name: "ids4_client_secrets");

            migrationBuilder.DropTable(
                name: "ids4_identity_resource_claims");

            migrationBuilder.DropTable(
                name: "ids4_identity_resource_properties");

            migrationBuilder.DropTable(
                name: "ids4_api_resources");

            migrationBuilder.DropTable(
                name: "ids4_api_scopes");

            migrationBuilder.DropTable(
                name: "ids4_clients");

            migrationBuilder.DropTable(
                name: "ids4_identity_resources");
        }
    }
}
