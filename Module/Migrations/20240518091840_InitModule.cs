using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Module.Migrations
{
    /// <inheritdoc />
    public partial class InitModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "module");

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
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
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
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
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
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspace", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceData",
                schema: "module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceData", x => x.Id);
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
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
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
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
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
                    Order = table.Column<int>(type: "int", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkspaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkspaceModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModuleId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkspaceId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
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
                        name: "FK_Property_Modules_ModuleId1",
                        column: x => x.ModuleId1,
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
                    table.ForeignKey(
                        name: "FK_Property_Workspace_WorkspaceId1",
                        column: x => x.WorkspaceId1,
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
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
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
                    StringValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IntValue = table.Column<int>(type: "int", nullable: true),
                    DateTimeValue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DoubleValue = table.Column<double>(type: "float", nullable: true),
                    DecimalValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BoolValue = table.Column<bool>(type: "bit", nullable: true),
                    ModuleDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkspaceDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkspaceModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_PropertyData_WorkspaceModules_WorkspaceModuleId",
                        column: x => x.WorkspaceModuleId,
                        principalSchema: "module",
                        principalTable: "WorkspaceModules",
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
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
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
                name: "IX_Property_ModuleId",
                schema: "module",
                table: "Property",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_ModuleId1",
                schema: "module",
                table: "Property",
                column: "ModuleId1");

            migrationBuilder.CreateIndex(
                name: "IX_Property_WorkspaceId",
                schema: "module",
                table: "Property",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_WorkspaceId1",
                schema: "module",
                table: "Property",
                column: "WorkspaceId1");

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
                name: "IX_PropertyData_WorkspaceModuleId",
                schema: "module",
                table: "PropertyData",
                column: "WorkspaceModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyFormulas_PropertyId",
                schema: "module",
                table: "PropertyFormulas",
                column: "PropertyId");

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
        }
    }
}
