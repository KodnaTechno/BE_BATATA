using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppMigration.SqlServer.Module
{
    /// <inheritdoc />
    public partial class AddDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "module",
                table: "WorkspaceModules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "module",
                table: "WorkspaceModules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "module",
                table: "WorkspaceModules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "module",
                table: "WorkspaceData",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "module",
                table: "WorkspaceData",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "module",
                table: "WorkspaceData",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowManySource",
                schema: "module",
                table: "WorkspaceConnection",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "module",
                table: "Workspace",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "module",
                table: "Workspace",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "module",
                table: "Workspace",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "module",
                table: "PropertyFormulas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "module",
                table: "PropertyFormulas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "module",
                table: "PropertyFormulas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "module",
                table: "PropertyData",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "module",
                table: "PropertyData",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "module",
                table: "PropertyData",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "module",
                table: "PropertyConnections",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "module",
                table: "PropertyConnections",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "module",
                table: "PropertyConnections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "module",
                table: "Property",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "module",
                table: "Property",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "module",
                table: "Property",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "module",
                table: "Modules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "module",
                table: "Modules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "module",
                table: "Modules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "module",
                table: "ModuleData",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "module",
                table: "ModuleData",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "module",
                table: "ModuleData",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "module",
                table: "ModuleBlocks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "module",
                table: "ModuleBlocks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "module",
                table: "ModuleBlocks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "module",
                table: "Applications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "module",
                table: "Applications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "module",
                table: "Applications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "module",
                table: "WorkspaceModules");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "module",
                table: "WorkspaceModules");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "module",
                table: "WorkspaceModules");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "module",
                table: "WorkspaceData");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "module",
                table: "WorkspaceData");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "module",
                table: "WorkspaceData");

            migrationBuilder.DropColumn(
                name: "AllowManySource",
                schema: "module",
                table: "WorkspaceConnection");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "module",
                table: "Workspace");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "module",
                table: "Workspace");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "module",
                table: "Workspace");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "module",
                table: "PropertyFormulas");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "module",
                table: "PropertyFormulas");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "module",
                table: "PropertyFormulas");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "module",
                table: "PropertyData");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "module",
                table: "PropertyData");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "module",
                table: "PropertyData");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "module",
                table: "PropertyConnections");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "module",
                table: "PropertyConnections");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "module",
                table: "PropertyConnections");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "module",
                table: "Property");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "module",
                table: "Property");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "module",
                table: "Property");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "module",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "module",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "module",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "module",
                table: "ModuleData");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "module",
                table: "ModuleData");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "module",
                table: "ModuleData");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "module",
                table: "ModuleBlocks");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "module",
                table: "ModuleBlocks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "module",
                table: "ModuleBlocks");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "module",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "module",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "module",
                table: "Applications");
        }
    }
}
