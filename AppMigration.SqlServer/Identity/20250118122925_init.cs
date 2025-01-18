using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppMigration.SqlServer.Identity
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.CreateTable(
                name: "appAccessibility",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleKey = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModuleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appAccessibility", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "appCredentials",
                schema: "identity",
                columns: table => new
                {
                    AppId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SecretKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OwnerEntity = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appCredentials", x => x.AppId);
                });

            migrationBuilder.CreateTable(
                name: "appGroups",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActivated = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "appRoles",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModuleType = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: true),
                    ExtraInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName_Ar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName_En = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "appUsers",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Stars = table.Column<int>(type: "int", nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ext = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsExternal = table.Column<bool>(type: "bit", nullable: false),
                    ExtraInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProviderExtraInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "appAccessibilityGroup",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppAccessibilityId = table.Column<int>(type: "int", nullable: false),
                    AppGroupId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appAccessibilityGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_appAccessibilityGroup_appAccessibility_AppAccessibilityId",
                        column: x => x.AppAccessibilityId,
                        principalSchema: "identity",
                        principalTable: "appAccessibility",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_appAccessibilityGroup_appGroups_AppGroupId",
                        column: x => x.AppGroupId,
                        principalSchema: "identity",
                        principalTable: "appGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "instanceGroupPermission",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstancePermissionId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_instanceGroupPermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_instanceGroupPermission_appGroups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "identity",
                        principalTable: "appGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "appPermissions",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModuleType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Command = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AppRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_appPermissions_appRoles_AppRoleId",
                        column: x => x.AppRoleId,
                        principalSchema: "identity",
                        principalTable: "appRoles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "appGroupUsers",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appGroupUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_appGroupUsers_appGroups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "identity",
                        principalTable: "appGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_appGroupUsers_appUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalSchema: "identity",
                        principalTable: "appUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_appGroupUsers_appUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "appUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "appUserClaims",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValueRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_appUserClaims_appUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "identity",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_appUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId, x.ModuleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_appRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "appRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_appUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_appUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "appGroupPermission",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppPermissionId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appGroupPermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_appGroupPermission_appGroups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "identity",
                        principalTable: "appGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_appGroupPermission_appPermissions_AppPermissionId",
                        column: x => x.AppPermissionId,
                        principalSchema: "identity",
                        principalTable: "appPermissions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "appRolePermission",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppPermissionId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appRolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_appRolePermission_appPermissions_AppPermissionId",
                        column: x => x.AppPermissionId,
                        principalSchema: "identity",
                        principalTable: "appPermissions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_appRolePermission_appRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "appRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_appAccessibility_ModuleKey",
                schema: "identity",
                table: "appAccessibility",
                column: "ModuleKey",
                unique: true,
                filter: "[ModuleKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_appAccessibilityGroup_AppAccessibilityId",
                schema: "identity",
                table: "appAccessibilityGroup",
                column: "AppAccessibilityId");

            migrationBuilder.CreateIndex(
                name: "IX_appAccessibilityGroup_AppGroupId",
                schema: "identity",
                table: "appAccessibilityGroup",
                column: "AppGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_appGroupPermission_AppPermissionId",
                schema: "identity",
                table: "appGroupPermission",
                column: "AppPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_appGroupPermission_GroupId",
                schema: "identity",
                table: "appGroupPermission",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_appGroupUsers_AppUserId",
                schema: "identity",
                table: "appGroupUsers",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_appGroupUsers_GroupId",
                schema: "identity",
                table: "appGroupUsers",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_appGroupUsers_UserId",
                schema: "identity",
                table: "appGroupUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_appPermissions_AppRoleId",
                schema: "identity",
                table: "appPermissions",
                column: "AppRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_appPermissions_Command_ModuleId",
                schema: "identity",
                table: "appPermissions",
                columns: new[] { "Command", "ModuleId" },
                unique: true,
                filter: "[Command] IS NOT NULL AND [ModuleId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_appRolePermission_AppPermissionId",
                schema: "identity",
                table: "appRolePermission",
                column: "AppPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_appRolePermission_RoleId",
                schema: "identity",
                table: "appRolePermission",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "identity",
                table: "appRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_appUserClaims_UserId",
                schema: "identity",
                table: "appUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "identity",
                table: "appUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "identity",
                table: "appUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "identity",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "identity",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_instanceGroupPermission_GroupId",
                schema: "identity",
                table: "instanceGroupPermission",
                column: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appAccessibilityGroup",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "appCredentials",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "appGroupPermission",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "appGroupUsers",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "appRolePermission",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "appUserClaims",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "instanceGroupPermission",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "appAccessibility",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "appPermissions",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "appUsers",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "appGroups",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "appRoles",
                schema: "identity");
        }
    }
}
