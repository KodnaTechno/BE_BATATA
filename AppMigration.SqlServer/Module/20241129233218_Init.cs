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
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "WorkspaceData",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "Workspace",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormlizedTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Workspace", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workspace_Applications_ApplicationId",
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
                name: "ModuleData",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModulId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                        name: "FK_ModuleData_Modules_ModulId",
                        column: x => x.ModulId,
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
                        name: "FK_WorkspaceConnection_Workspace_SourceWorkspaceId",
                        column: x => x.SourceWorkspaceId,
                        principalSchema: "module",
                        principalTable: "Workspace",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkspaceConnection_Workspace_TargetWorkspaceId",
                        column: x => x.TargetWorkspaceId,
                        principalSchema: "module",
                        principalTable: "Workspace",
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
                        name: "FK_WorkspaceModuleBlocks_Workspace_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalSchema: "module",
                        principalTable: "Workspace",
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
                        name: "FK_WorkspaceModules_Workspace_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalSchema: "module",
                        principalTable: "Workspace",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkspaceConnectionData_WorkspaceData_TargetWorkspaceDataId",
                        column: x => x.TargetWorkspaceDataId,
                        principalSchema: "module",
                        principalTable: "WorkspaceData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Property",
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
                    table.PrimaryKey("PK_Property", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Property_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "module",
                        principalTable: "Modules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Property_WorkspaceModules_WorkspaceModuleId",
                        column: x => x.WorkspaceModuleId,
                        principalSchema: "module",
                        principalTable: "WorkspaceModules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Property_Workspace_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalSchema: "module",
                        principalTable: "Workspace",
                        principalColumn: "Id");
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
                        name: "FK_PropertyConnections_Property_SourcePropertyId",
                        column: x => x.SourcePropertyId,
                        principalSchema: "module",
                        principalTable: "Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyConnections_Property_TargetPropertyId",
                        column: x => x.TargetPropertyId,
                        principalSchema: "module",
                        principalTable: "Property",
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
                    DataType = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_PropertyData_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalSchema: "module",
                        principalTable: "Property",
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
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyFormulas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyFormulas_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalSchema: "module",
                        principalTable: "Property",
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
                        name: "FK_ValidationRules_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalSchema: "module",
                        principalTable: "Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "module",
                table: "Modules",
                columns: new[] { "Id", "ApplicationId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Details", "Domain", "IsActive", "IsDeleted", "Name", "Order", "Title", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("11111111-1111-1111-1111-111111111111"), null, null, "{\r\n  \"en\": \"تفاصيل\",\r\n  \"ar\": \"Details\"\r\n}", "Module.Domain.BusinessDomain.Task", true, false, "Task", 1, "{\r\n  \"en\": \"المهام\",\r\n  \"ar\": \"Tasks\"\r\n}", "Basic", null, null });

            migrationBuilder.InsertData(
                schema: "module",
                table: "Property",
                columns: new[] { "Id", "Configuration", "CreatedAt", "CreatedBy", "DataType", "DefaultValue", "DeletedAt", "DeletedBy", "Description", "IsCalculated", "IsDeleted", "IsEncrypted", "IsInternal", "IsSystem", "IsTranslatable", "Key", "ModuleId", "NormalizedKey", "Order", "SystemPropertyPath", "Title", "UpdatedAt", "UpdatedBy", "ViewType", "WorkspaceId", "WorkspaceModuleId" },
                values: new object[,]
                {
                    { new Guid("1894b31a-c4c4-411e-b116-e3d3ea0d5124"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("11111111-1111-1111-1111-111111111111"), "DateTime", null, null, null, null, true, false, false, true, true, false, "task_updatedat", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "UPDATED_AT", 6, "UpdatedAt", "{\r\n  \"en\": \"Updated At\",\r\n  \"ar\": \"تاريخ التحديث\"\r\n}", null, null, "DateTime", null, null },
                    { new Guid("63e6128a-2903-4500-a2e8-15af07867df3"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("11111111-1111-1111-1111-111111111111"), "DateTime", null, null, null, null, true, false, false, true, true, false, "task_createdat", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "CREATED_AT", 4, "CreatedAt", "{\r\n  \"en\": \"Created At\",\r\n  \"ar\": \"تاريخ الإنشاء\"\r\n}", null, null, "DateTime", null, null },
                    { new Guid("64b8369e-497f-462d-bc30-ac97c3e43b30"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("11111111-1111-1111-1111-111111111111"), "Guid", null, null, null, null, true, false, false, true, true, false, "task_deletedby", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "DELETED_BY", 9, "DeletedBy", "{\r\n  \"en\": \"Deleted By\",\r\n  \"ar\": \"تم الحذف بواسطة\"\r\n}", null, null, "User", null, null },
                    { new Guid("81cc79f8-200b-49bc-ac71-b6d2e19b4cc4"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("11111111-1111-1111-1111-111111111111"), "Guid", null, null, null, null, true, false, false, true, true, false, "task_updatedby", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "UPDATED_BY", 7, "UpdatedBy", "{\r\n  \"en\": \"Updated By\",\r\n  \"ar\": \"تم التحديث بواسطة\"\r\n}", null, null, "User", null, null },
                    { new Guid("9d6b2976-c5ea-4c7a-91e7-c684f3b57f33"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("11111111-1111-1111-1111-111111111111"), "String", null, null, null, null, false, false, false, true, true, true, "task_title", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "TITLE", 1, "Title", "{\r\n  \"en\": \"Title\",\r\n  \"ar\": \"العنوان\"\r\n}", null, null, "Text", null, null },
                    { new Guid("b653054d-75a9-4c48-9fe8-c5704459e578"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("11111111-1111-1111-1111-111111111111"), "DateOnly", null, null, null, null, false, false, false, true, true, false, "task_duedate", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "DUEDATE", 3, "DueDate", "{\r\n  \"en\": \"Due Date\",\r\n  \"ar\": \"تاريخ الاستحقاق\"\r\n}", null, null, "Date", null, null },
                    { new Guid("e0a3bbff-5314-41fe-9a9d-5b13b2151a67"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("11111111-1111-1111-1111-111111111111"), "Guid", null, null, null, null, true, false, false, true, true, false, "task_createdby", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "CREATED_BY", 5, "CreatedBy", "{\r\n  \"en\": \"Created By\",\r\n  \"ar\": \"تم الإنشاء بواسطة\"\r\n}", null, null, "User", null, null },
                    { new Guid("ee82a724-8aa7-412d-add7-cfc25b4d15f6"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("11111111-1111-1111-1111-111111111111"), "DateTime", null, null, null, null, true, false, false, true, true, false, "task_deletedat", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "DELETED_AT", 8, "DeletedAt", "{\r\n  \"en\": \"Deleted At\",\r\n  \"ar\": \"تاريخ الحذف\"\r\n}", null, null, "DateTime", null, null },
                    { new Guid("f1f61de5-c906-4a0e-8a79-37a119fb6a54"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("11111111-1111-1111-1111-111111111111"), "Guid", null, null, null, null, false, false, false, true, true, false, "task_assignedto", new Guid("89a9748e-41d5-4c31-9c5c-52a10c4f7419"), "ASSIGNDTO", 2, "AssigndTo", "{\r\n  \"en\": \"Assigned To\",\r\n  \"ar\": \"مسند إلى\"\r\n}", null, null, "User", null, null }
                });

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
                name: "IX_ModuleData_ModulId",
                schema: "module",
                table: "ModuleData",
                column: "ModulId");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_ApplicationId",
                schema: "module",
                table: "Modules",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_ModuleId",
                schema: "module",
                table: "Property",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_WorkspaceId",
                schema: "module",
                table: "Property",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_WorkspaceModuleId",
                schema: "module",
                table: "Property",
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
                name: "IX_Workspace_ApplicationId",
                schema: "module",
                table: "Workspace",
                column: "ApplicationId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "Property",
                schema: "module");

            migrationBuilder.DropTable(
                name: "WorkspaceConnection",
                schema: "module");

            migrationBuilder.DropTable(
                name: "WorkspaceData",
                schema: "module");

            migrationBuilder.DropTable(
                name: "ModuleBlocks",
                schema: "module");

            migrationBuilder.DropTable(
                name: "WorkspaceModules",
                schema: "module");

            migrationBuilder.DropTable(
                name: "Modules",
                schema: "module");

            migrationBuilder.DropTable(
                name: "Workspace",
                schema: "module");

            migrationBuilder.DropTable(
                name: "Applications",
                schema: "module");
        }
    }
}
