using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppMigration.SqlServer.Module
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "module");

            migrationBuilder.CreateTable(
                name: "Applications",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModuleBlocks",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleBlocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modules_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "module",
                        principalTable: "Applications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Workspaces",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workspaces_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "module",
                        principalTable: "Applications",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ModuleBlockModules",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleBlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleBlockModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleBlockModules_ModuleBlocks_ModuleBlockId",
                        column: x => x.ModuleBlockId,
                        principalSchema: "module",
                        principalTable: "ModuleBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModuleBlockModules_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "module",
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceConnection",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceWorkspaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetWorkspaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    IsOptional = table.Column<bool>(type: "bit", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowManySource = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceConnection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkspaceConnection_Workspaces_SourceWorkspaceId",
                        column: x => x.SourceWorkspaceId,
                        principalSchema: "module",
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkspaceConnection_Workspaces_TargetWorkspaceId",
                        column: x => x.TargetWorkspaceId,
                        principalSchema: "module",
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceData",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkspaceData_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalSchema: "module",
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceModuleBlocks",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleBlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceModuleBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkspaceModuleBlocks_ModuleBlocks_ModuleBlockId",
                        column: x => x.ModuleBlockId,
                        principalSchema: "module",
                        principalTable: "ModuleBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceModuleBlocks_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalSchema: "module",
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceModules",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkspaceModules_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "module",
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceModules_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalSchema: "module",
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModuleData",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkSpaceDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleData_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "module",
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModuleData_WorkspaceData_WorkSpaceDataId",
                        column: x => x.WorkSpaceDataId,
                        principalSchema: "module",
                        principalTable: "WorkspaceData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceConnectionData",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceWorkspaceDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetWorkspaceDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkspaceConnectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceConnectionData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkspaceConnectionData_WorkspaceConnection_WorkspaceConnectionId",
                        column: x => x.WorkspaceConnectionId,
                        principalSchema: "module",
                        principalTable: "WorkspaceConnection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceConnectionData_WorkspaceData_SourceWorkspaceDataId",
                        column: x => x.SourceWorkspaceDataId,
                        principalSchema: "module",
                        principalTable: "WorkspaceData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkspaceConnectionData_WorkspaceData_TargetWorkspaceDataId",
                        column: x => x.TargetWorkspaceDataId,
                        principalSchema: "module",
                        principalTable: "WorkspaceData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppActions",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkspaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ValidationFormula = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkspaceModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppActions_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "module",
                        principalTable: "Modules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppActions_WorkspaceModules_WorkspaceModuleId",
                        column: x => x.WorkspaceModuleId,
                        principalSchema: "module",
                        principalTable: "WorkspaceModules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppActions_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalSchema: "module",
                        principalTable: "Workspaces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViewType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    IsInternal = table.Column<bool>(type: "bit", nullable: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCalculated = table.Column<bool>(type: "bit", nullable: false),
                    IsEncrypted = table.Column<bool>(type: "bit", nullable: false),
                    IsTranslatable = table.Column<bool>(type: "bit", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkspaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkspaceModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SystemPropertyPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Properties_Applications_WorkspaceModuleId",
                        column: x => x.WorkspaceModuleId,
                        principalSchema: "module",
                        principalTable: "Applications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Properties_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "module",
                        principalTable: "Modules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Properties_WorkspaceModules_WorkspaceModuleId",
                        column: x => x.WorkspaceModuleId,
                        principalSchema: "module",
                        principalTable: "WorkspaceModules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Properties_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalSchema: "module",
                        principalTable: "Workspaces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_ModuleData_ModuleDataId",
                        column: x => x.ModuleDataId,
                        principalSchema: "module",
                        principalTable: "ModuleData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyConnections",
                schema: "module",
                columns: table => new
                {
                    SourcePropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetPropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConnectionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyConnections", x => new { x.SourcePropertyId, x.TargetPropertyId });
                    table.ForeignKey(
                        name: "FK_PropertyConnections_Properties_SourcePropertyId",
                        column: x => x.SourcePropertyId,
                        principalSchema: "module",
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyConnections_Properties_TargetPropertyId",
                        column: x => x.TargetPropertyId,
                        principalSchema: "module",
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropertyData",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ViewType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SystemPropertyPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuidValue = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StringValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IntValue = table.Column<int>(type: "int", nullable: true),
                    DateTimeValue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateValue = table.Column<DateOnly>(type: "date", nullable: true),
                    DoubleValue = table.Column<double>(type: "float", nullable: true),
                    DecimalValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BoolValue = table.Column<bool>(type: "bit", nullable: true),
                    ModuleDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkspaceDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyData_ModuleData_ModuleDataId",
                        column: x => x.ModuleDataId,
                        principalSchema: "module",
                        principalTable: "ModuleData",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropertyData_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalSchema: "module",
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyData_WorkspaceData_WorkspaceDataId",
                        column: x => x.WorkspaceDataId,
                        principalSchema: "module",
                        principalTable: "WorkspaceData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PropertyFormulas",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Formula = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsConditional = table.Column<bool>(type: "bit", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyFormulas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyFormulas_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalSchema: "module",
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValidationRules",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RuleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RuleValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValidationRules_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalSchema: "module",
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "module",
                table: "Applications",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "Icon", "IsDeleted", "Key", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("a1b2c3d4-e5f6-47ae-8eb7-d1e1e83a6f9c"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("11111111-1111-1111-1111-111111111111"), null, null, "{\r\n  \"en\": \"Manage all your organizational assets and their details.\",\r\n  \"ar\": \"إدارة جميع أصول مؤسستك وتفاصيلها\"\r\n}", "fa-solid fa-warehouse", false, null, "{\"En\":\"Asset Management\",\"Ar\":\"\\u0625\\u062F\\u0627\\u0631\\u0647 \\u0627\\u0644\\u0645\\u0631\\u0627\\u0641\\u0642\"}", null, null });

            migrationBuilder.InsertData(
                schema: "module",
                table: "Modules",
                columns: new[] { "Id", "ApplicationId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Details", "Domain", "IsActive", "IsDeleted", "Key", "Order", "Title", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), new Guid("a1b2c3d4-e5f6-47ae-8eb7-d1e1e83a6f9c"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), null, null, "{\"En\":\"\\u062A\\u0641\\u0627\\u0635\\u064A\\u0644\",\"Ar\":\"Details\"}", "Module.Domain.BusinessDomain.Task", true, false, "Task", 1, "{\"En\":\"\\u0627\\u0644\\u0645\\u0647\\u0627\\u0645\",\"Ar\":\"Tasks\"}", "Basic", null, null });

            migrationBuilder.InsertData(
                schema: "module",
                table: "Workspaces",
                columns: new[] { "Id", "ApplicationId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Details", "IsDeleted", "Key", "Order", "Title", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("e9a8748e-41d5-4c31-9c5c-52a10c4f7420"), new Guid("a1b2c3d4-e5f6-47ae-8eb7-d1e1e83a6f9c"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), null, null, "{\"En\":\"Project\",\"Ar\":\"\\u0645\\u0634\\u0631\\u0648\\u0639\"}", false, "PROJECT", 1, "{\"En\":\"Project\",\"Ar\":\"\\u0645\\u0634\\u0631\\u0648\\u0639\"}", "Basic", null, null });

            migrationBuilder.InsertData(
                schema: "module",
                table: "AppActions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsDeleted", "ModuleId", "Name", "Type", "UpdatedAt", "UpdatedBy", "ValidationFormula", "WorkspaceId", "WorkspaceModuleId" },
                values: new object[,]
                {
                    { new Guid("62e6138a-2903-4500-a2e8-15af07867df3"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), null, null, "{\"En\":\"Update\",\"Ar\":\"\\u062A\\u0639\\u062F\\u064A\\u0644\"}", false, new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "{\"En\":\"Update\",\"Ar\":\"\\u062A\\u0639\\u062F\\u064A\\u0644\"}", "Update", null, null, null, null, null },
                    { new Guid("63e6138a-2903-4500-a2e8-15af07867df3"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), null, null, "{\"En\":\"Create\",\"Ar\":\"\\u0627\\u0636\\u0627\\u0641\\u0629\"}", false, new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "{\"En\":\"Create\",\"Ar\":\"\\u0627\\u0636\\u0627\\u0641\\u0629\"}", "Create", null, null, null, null, null },
                    { new Guid("65e6118a-2903-4500-a2e8-15af07867df3"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), null, null, "{\"En\":\"Delete\",\"Ar\":\"\\u062D\\u0630\\u0641\"}", false, new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "{\"En\":\"Delete\",\"Ar\":\"\\u062D\\u0630\\u0641\"}", "Delete", null, null, null, null, null },
                    { new Guid("66e6118a-2903-4500-a2e8-15af07867df3"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), null, null, "{\"En\":\"Read\",\"Ar\":\"\\u0645\\u0639\\u0627\\u064A\\u0646\\u0629\"}", false, new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "{\"En\":\"Read\",\"Ar\":\"\\u0645\\u0639\\u0627\\u064A\\u0646\\u0629\"}", "Read", null, null, null, null, null }
                });

            migrationBuilder.InsertData(
                schema: "module",
                table: "Properties",
                columns: new[] { "Id", "ApplicationId", "Configuration", "CreatedAt", "CreatedBy", "DataType", "DefaultValue", "DeletedAt", "DeletedBy", "Description", "IsCalculated", "IsDeleted", "IsEncrypted", "IsInternal", "IsSystem", "IsTranslatable", "Key", "ModuleId", "NormalizedKey", "Order", "SystemPropertyPath", "Title", "UpdatedAt", "UpdatedBy", "ViewType", "WorkspaceId", "WorkspaceModuleId" },
                values: new object[,]
                {
                    { new Guid("1894b31a-c4c4-411e-b116-e3d3ea0d5124"), null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), "DateTime", null, null, null, null, true, false, false, true, true, false, "task_updated_at", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "UPDATED_AT", 6, "UpdatedAt", "{\"En\":\"Updated At\",\"Ar\":\"\\u062A\\u0627\\u0631\\u064A\\u062E \\u0627\\u0644\\u062A\\u062D\\u062F\\u064A\\u062B\"}", null, null, "DateTime", null, null },
                    { new Guid("2224fae6-3dff-45c9-8fd2-6996fb14e9e0"), null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), "DateTime", null, null, null, null, true, false, false, true, true, false, "project_updated_at", null, "UPDATED_AT", 10, "UpdatedAt", "{\"En\":\"Updated At\",\"Ar\":\"\\u062A\\u0627\\u0631\\u064A\\u062E \\u0627\\u0644\\u062A\\u062D\\u062F\\u064A\\u062B\"}", null, null, "DateTime", new Guid("e9a8748e-41d5-4c31-9c5c-52a10c4f7420"), null },
                    { new Guid("2392d348-6af0-4b67-9271-ba6a8aeebed0"), null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), "DateTime", null, null, null, null, true, false, false, true, true, false, "project_created_at", null, "CREATED_AT", 8, "CreatedAt", "{\"En\":\"Created At\",\"Ar\":\"\\u062A\\u0627\\u0631\\u064A\\u062E \\u0627\\u0644\\u0625\\u0646\\u0634\\u0627\\u0621\"}", null, null, "DateTime", new Guid("e9a8748e-41d5-4c31-9c5c-52a10c4f7420"), null },
                    { new Guid("63e6128a-2903-4500-a2e8-15af07867df3"), null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), "DateTime", null, null, null, null, true, false, false, true, true, false, "task_created_at", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "CREATED_AT", 4, "CreatedAt", "{\"En\":\"Created At\",\"Ar\":\"\\u062A\\u0627\\u0631\\u064A\\u062E \\u0627\\u0644\\u0625\\u0646\\u0634\\u0627\\u0621\"}", null, null, "DateTime", null, null },
                    { new Guid("64b8369e-497f-462d-bc30-ac97c3e43b30"), null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), "Guid", null, null, null, null, true, false, false, true, true, false, "task_deleted_by", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "DELETED_BY", 9, "DeletedBy", "{\"En\":\"Deleted By\",\"Ar\":\"\\u062A\\u0645 \\u0627\\u0644\\u062D\\u0630\\u0641 \\u0628\\u0648\\u0627\\u0633\\u0637\\u0629\"}", null, null, "User", null, null },
                    { new Guid("71c58090-d4c0-4bee-8b9b-417f938de7f4"), null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), "DateTime", null, null, null, null, true, false, false, true, true, false, "project_deleted_at", null, "DELETED_AT", 12, "DeletedAt", "{\"En\":\"Deleted At\",\"Ar\":\"\\u062A\\u0627\\u0631\\u064A\\u062E \\u0627\\u0644\\u062D\\u0630\\u0641\"}", null, null, "DateTime", new Guid("e9a8748e-41d5-4c31-9c5c-52a10c4f7420"), null },
                    { new Guid("81cc79f8-200b-49bc-ac71-b6d2e19b4cc4"), null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), "Guid", null, null, null, null, true, false, false, true, true, false, "task_updated_by", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "UPDATED_BY", 7, "UpdatedBy", "{\"En\":\"Updated By\",\"Ar\":\"\\u062A\\u0645 \\u0627\\u0644\\u062A\\u062D\\u062F\\u064A\\u062B \\u0628\\u0648\\u0627\\u0633\\u0637\\u0629\"}", null, null, "User", null, null },
                    { new Guid("9d6b2976-c5ea-4c7a-91e7-c684f3b57f33"), null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), "String", null, null, null, null, false, false, false, true, true, true, "task_title", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "TITLE", 1, "Title", "{\"En\":\"Title\",\"Ar\":\"\\u0627\\u0644\\u0639\\u0646\\u0648\\u0627\\u0646\"}", null, null, "Text", null, null },
                    { new Guid("a73e4ce5-50b6-4a79-a979-81722b6d4352"), null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), "Guid", null, null, null, null, true, false, false, true, true, false, "project_updated_by", null, "UPDATED_BY", 11, "UpdatedBy", "{\"En\":\"Updated By\",\"Ar\":\"\\u062A\\u0645 \\u0627\\u0644\\u062A\\u062D\\u062F\\u064A\\u062B \\u0628\\u0648\\u0627\\u0633\\u0637\\u0629\"}", null, null, "User", new Guid("e9a8748e-41d5-4c31-9c5c-52a10c4f7420"), null },
                    { new Guid("b653054d-75a9-4c48-9fe8-c5704459e578"), null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), "DateOnly", null, null, null, null, false, false, false, true, true, false, "task_duedate", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "DUEDATE", 3, "DueDate", "{\"En\":\"Due Date\",\"Ar\":\"\\u062A\\u0627\\u0631\\u064A\\u062E \\u0627\\u0644\\u0627\\u0633\\u062A\\u062D\\u0642\\u0627\\u0642\"}", null, null, "Date", null, null },
                    { new Guid("e0a3bbff-5314-41fe-9a9d-5b13b2151a67"), null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), "Guid", null, null, null, null, true, false, false, true, true, false, "task_created_by", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "CREATED_BY", 5, "CreatedBy", "{\"En\":\"Created By\",\"Ar\":\"\\u062A\\u0645 \\u0627\\u0644\\u0625\\u0646\\u0634\\u0627\\u0621 \\u0628\\u0648\\u0627\\u0633\\u0637\\u0629\"}", null, null, "User", null, null },
                    { new Guid("ee82a724-8aa7-412d-add7-cfc25b4d15f6"), null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), "DateTime", null, null, null, null, true, false, false, true, true, false, "task_deleted_at", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "DELETED_AT", 8, "DeletedAt", "{\"En\":\"Deleted At\",\"Ar\":\"\\u062A\\u0627\\u0631\\u064A\\u062E \\u0627\\u0644\\u062D\\u0630\\u0641\"}", null, null, "DateTime", null, null },
                    { new Guid("f1f61de5-c906-4a0e-8a79-37a119fb6a54"), null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), "Guid", null, null, null, null, false, false, false, true, true, false, "task_assignedto", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "ASSIGNDTO", 2, "AssigndTo", "{\"En\":\"Assigned To\",\"Ar\":\"\\u0645\\u0633\\u0646\\u062F \\u0625\\u0644\\u0649\"}", null, null, "User", null, null },
                    { new Guid("f2a4b262-5e35-4ce7-98ca-e4af8c08cc60"), null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), "Guid", null, null, null, null, true, false, false, true, true, false, "project_deleted_by", null, "DELETED_BY", 13, "DeletedBy", "{\"En\":\"Deleted By\",\"Ar\":\"\\u062A\\u0645 \\u0627\\u0644\\u062D\\u0630\\u0641 \\u0628\\u0648\\u0627\\u0633\\u0637\\u0629\"}", null, null, "User", new Guid("e9a8748e-41d5-4c31-9c5c-52a10c4f7420"), null },
                    { new Guid("fb23f579-e069-4ecc-bbfd-58ebe8dd2350"), null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000002"), "Guid", null, null, null, null, true, false, false, true, true, false, "project_created_by", null, "CREATED_BY", 9, "CreatedBy", "{\"En\":\"Created By\",\"Ar\":\"\\u062A\\u0645 \\u0627\\u0644\\u0625\\u0646\\u0634\\u0627\\u0621 \\u0628\\u0648\\u0627\\u0633\\u0637\\u0629\"}", null, null, "User", new Guid("e9a8748e-41d5-4c31-9c5c-52a10c4f7420"), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppActions_ModuleId",
                schema: "module",
                table: "AppActions",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_AppActions_WorkspaceId",
                schema: "module",
                table: "AppActions",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_AppActions_WorkspaceModuleId",
                schema: "module",
                table: "AppActions",
                column: "WorkspaceModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleBlockModules_ModuleBlockId",
                schema: "module",
                table: "ModuleBlockModules",
                column: "ModuleBlockId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleBlockModules_ModuleId",
                schema: "module",
                table: "ModuleBlockModules",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleData_ModuleId",
                schema: "module",
                table: "ModuleData",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleData_WorkSpaceDataId",
                schema: "module",
                table: "ModuleData",
                column: "WorkSpaceDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_ApplicationId",
                schema: "module",
                table: "Modules",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_ModuleId",
                schema: "module",
                table: "Properties",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_WorkspaceId",
                schema: "module",
                table: "Properties",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_WorkspaceModuleId",
                schema: "module",
                table: "Properties",
                column: "WorkspaceModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyConnections_TargetPropertyId",
                schema: "module",
                table: "PropertyConnections",
                column: "TargetPropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyData_ModuleDataId",
                schema: "module",
                table: "PropertyData",
                column: "ModuleDataId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyData_PropertyId",
                schema: "module",
                table: "PropertyData",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyData_WorkspaceDataId",
                schema: "module",
                table: "PropertyData",
                column: "WorkspaceDataId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyFormulas_PropertyId",
                schema: "module",
                table: "PropertyFormulas",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ModuleDataId",
                schema: "module",
                table: "Tasks",
                column: "ModuleDataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ValidationRules_PropertyId",
                schema: "module",
                table: "ValidationRules",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceConnection_SourceWorkspaceId",
                schema: "module",
                table: "WorkspaceConnection",
                column: "SourceWorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceConnection_TargetWorkspaceId",
                schema: "module",
                table: "WorkspaceConnection",
                column: "TargetWorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceConnectionData_SourceWorkspaceDataId",
                schema: "module",
                table: "WorkspaceConnectionData",
                column: "SourceWorkspaceDataId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceConnectionData_TargetWorkspaceDataId",
                schema: "module",
                table: "WorkspaceConnectionData",
                column: "TargetWorkspaceDataId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceConnectionData_WorkspaceConnectionId",
                schema: "module",
                table: "WorkspaceConnectionData",
                column: "WorkspaceConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceData_WorkspaceId",
                schema: "module",
                table: "WorkspaceData",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceModuleBlocks_ModuleBlockId",
                schema: "module",
                table: "WorkspaceModuleBlocks",
                column: "ModuleBlockId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceModuleBlocks_WorkspaceId",
                schema: "module",
                table: "WorkspaceModuleBlocks",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceModules_ModuleId",
                schema: "module",
                table: "WorkspaceModules",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceModules_WorkspaceId",
                schema: "module",
                table: "WorkspaceModules",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_ApplicationId",
                schema: "module",
                table: "Workspaces",
                column: "ApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppActions",
                schema: "module");

            migrationBuilder.DropTable(
                name: "ModuleBlockModules",
                schema: "module");

            migrationBuilder.DropTable(
                name: "PropertyConnections",
                schema: "module");

            migrationBuilder.DropTable(
                name: "PropertyData",
                schema: "module");

            migrationBuilder.DropTable(
                name: "PropertyFormulas",
                schema: "module");

            migrationBuilder.DropTable(
                name: "Tasks",
                schema: "module");

            migrationBuilder.DropTable(
                name: "ValidationRules",
                schema: "module");

            migrationBuilder.DropTable(
                name: "WorkspaceConnectionData",
                schema: "module");

            migrationBuilder.DropTable(
                name: "WorkspaceModuleBlocks",
                schema: "module");

            migrationBuilder.DropTable(
                name: "ModuleData",
                schema: "module");

            migrationBuilder.DropTable(
                name: "Properties",
                schema: "module");

            migrationBuilder.DropTable(
                name: "WorkspaceConnection",
                schema: "module");

            migrationBuilder.DropTable(
                name: "ModuleBlocks",
                schema: "module");

            migrationBuilder.DropTable(
                name: "WorkspaceData",
                schema: "module");

            migrationBuilder.DropTable(
                name: "WorkspaceModules",
                schema: "module");

            migrationBuilder.DropTable(
                name: "Modules",
                schema: "module");

            migrationBuilder.DropTable(
                name: "Workspaces",
                schema: "module");

            migrationBuilder.DropTable(
                name: "Applications",
                schema: "module");
        }
    }
}
