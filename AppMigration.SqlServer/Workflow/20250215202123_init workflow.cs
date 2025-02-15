using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppMigration.SqlServer.Workflow
{
    /// <inheritdoc />
    public partial class initworkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovalRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedProperties = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApproverIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EditableProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModuleData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovalMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProcessedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SynchronizationPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequiredStepIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompletedStepIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SynchronizationPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowCheckpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Variables = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckpointTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StepData = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowCheckpoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CurrentStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Variables = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AuditLog = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workflows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ModuleType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InitialStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsLatestVersion = table.Column<bool>(type: "bit", nullable: false),
                    Variables = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertiesKeys = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timeout = table.Column<TimeSpan>(type: "time", nullable: true),
                    RetryPolicy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AuditLog = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TriggerConfigs = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChildInstanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelationType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowRelations_WorkflowInstances_ChildInstanceId",
                        column: x => x.ChildInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowRelations_WorkflowInstances_ParentInstanceId",
                        column: x => x.ParentInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowStepInstances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    StepVariables = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InputData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowStepInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowStepInstances_WorkflowInstances_WorkflowDataId",
                        column: x => x.WorkflowDataId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActionConfiguration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timeout = table.Column<TimeSpan>(type: "time", nullable: true),
                    RetryPolicy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsParallel = table.Column<bool>(type: "bit", nullable: false),
                    ParallelSteps = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowSteps_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StepExecutionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkflowStepDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepExecutionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepExecutionLogs_WorkflowStepInstances_WorkflowStepDataId",
                        column: x => x.WorkflowStepDataId,
                        principalTable: "WorkflowStepInstances",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StepTransitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkflowStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepTransitions_WorkflowSteps_WorkflowStepId",
                        column: x => x.WorkflowStepId,
                        principalTable: "WorkflowSteps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalHistories_ApprovalRequestId",
                table: "ApprovalHistories",
                column: "ApprovalRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalHistories_Timestamp",
                table: "ApprovalHistories",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalHistories_UserId",
                table: "ApprovalHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_ExpiresAt",
                table: "ApprovalRequests",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_Status",
                table: "ApprovalRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_WorkflowDataId",
                table: "ApprovalRequests",
                column: "WorkflowDataId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Entity",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StepExecutionLogs_WorkflowStepDataId",
                table: "StepExecutionLogs",
                column: "WorkflowStepDataId");

            migrationBuilder.CreateIndex(
                name: "IX_StepTransitions_FromTo",
                table: "StepTransitions",
                columns: new[] { "SourceStepId", "TargetStepId" });

            migrationBuilder.CreateIndex(
                name: "IX_StepTransitions_WorkflowStepId",
                table: "StepTransitions",
                column: "WorkflowStepId");

            migrationBuilder.CreateIndex(
                name: "IX_SynchronizationPoints_Instance_Name",
                table: "SynchronizationPoints",
                columns: new[] { "WorkflowInstanceId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SynchronizationPoints_WorkflowInstanceId",
                table: "SynchronizationPoints",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowCheckpoints_CheckpointTime",
                table: "WorkflowCheckpoints",
                column: "CheckpointTime");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowCheckpoints_InstanceId",
                table: "WorkflowCheckpoints",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_StartedAt",
                table: "WorkflowInstances",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_Status",
                table: "WorkflowInstances",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_WorkflowId",
                table: "WorkflowInstances",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowRelations_Child",
                table: "WorkflowRelations",
                columns: new[] { "ChildInstanceId", "RelationType" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowRelations_Parent",
                table: "WorkflowRelations",
                columns: new[] { "ParentInstanceId", "RelationType" });

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_IsLatestVersion",
                table: "Workflows",
                column: "IsLatestVersion");

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_ModuleType_Status",
                table: "Workflows",
                columns: new[] { "ModuleType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_Version",
                table: "Workflows",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStepInstances_Status",
                table: "WorkflowStepInstances",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStepInstances_WorkflowInstance_Step",
                table: "WorkflowStepInstances",
                columns: new[] { "WorkflowDataId", "StepId" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_ActionType",
                table: "WorkflowSteps",
                column: "ActionType");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_WorkflowId",
                table: "WorkflowSteps",
                column: "WorkflowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalHistories");

            migrationBuilder.DropTable(
                name: "ApprovalRequests");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "StepExecutionLogs");

            migrationBuilder.DropTable(
                name: "StepTransitions");

            migrationBuilder.DropTable(
                name: "SynchronizationPoints");

            migrationBuilder.DropTable(
                name: "WorkflowCheckpoints");

            migrationBuilder.DropTable(
                name: "WorkflowRelations");

            migrationBuilder.DropTable(
                name: "WorkflowStepInstances");

            migrationBuilder.DropTable(
                name: "WorkflowSteps");

            migrationBuilder.DropTable(
                name: "WorkflowInstances");

            migrationBuilder.DropTable(
                name: "Workflows");
        }
    }
}
