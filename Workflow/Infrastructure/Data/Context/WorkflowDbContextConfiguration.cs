//using AppWorkflow.Core.Domain.Data;
//using AppWorkflow.Core.Domain.Schema;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AppWorkflow.Infrastructure.Data.Context
//{
//    public static partial class WorkflowDbContextConfiguration
//    {
//        public static void ConfigureWorkflowRelations(this ModelBuilder modelBuilder)
//        {


//            modelBuilder.Entity<Workflow>(entity =>
//            {
//                entity.ToTable("Workflows");
//                entity.HasKey(e => e.Id);

//                entity.Property(e => e.Name)
//                    .IsRequired()
//                    .HasMaxLength(200);

//                entity.Property(e => e.Description)
//                    .HasMaxLength(1000);

//                entity.Property(e => e.ModuleType)
//                    .IsRequired()
//                    .HasMaxLength(100);

//                entity.Property(e => e.Version)
//                    .IsRequired()
//                    .HasMaxLength(50);

//                entity.Property(e => e.TriggerConfigs)

//                entity.Property(e => e.Metadata)
//                    .HasColumnType("jsonb");

//                entity.HasIndex(e => new { e.ModuleType, e.Status })
//                    .HasDatabaseName("IX_Workflows_ModuleType_Status");

//                entity.HasIndex(e => e.Version)
//                    .HasDatabaseName("IX_Workflows_Version");

//                entity.HasIndex(e => e.IsLatestVersion)
//                    .HasDatabaseName("IX_Workflows_IsLatestVersion");

//                entity.HasQueryFilter(w => !w.IsDeleted);
//            });

//            modelBuilder.Entity<WorkflowStep>(entity =>
//            {
//                entity.ToTable("WorkflowSteps");
//                entity.HasKey(e => e.Id);

//                entity.Property(e => e.Name)
//                    .IsRequired()
//                    .HasMaxLength(200);

//                entity.Property(e => e.ActionType)
//                    .IsRequired()
//                    .HasMaxLength(100);

//                entity.Property(e => e.ActionConfiguration)
//                    .HasColumnType("jsonb");

//                entity.Property(e => e.Metadata)
//                    .HasColumnType("jsonb");

//                entity.HasIndex(e => e.WorkflowId)
//                    .HasDatabaseName("IX_WorkflowSteps_WorkflowId");

//                entity.HasIndex(e => e.ActionType)
//                    .HasDatabaseName("IX_WorkflowSteps_ActionType");

//                entity.HasOne<Workflow>()
//                    .WithMany(w => w.Steps)
//                    .HasForeignKey(s => s.WorkflowId)
//                    .OnDelete(DeleteBehavior.Cascade);
//            });

//            modelBuilder.Entity<WorkflowData>(entity =>
//            {
//                entity.ToTable("WorkflowInstances");
//                entity.HasKey(e => e.Id);

//                entity.Property(e => e.WorkflowVersion)
//                    .IsRequired()
//                    .HasMaxLength(50);

//                entity.Property(e => e.ModuleData)
//                    .HasColumnType("jsonb");

//                entity.Property(e => e.Variables)
//                    .HasColumnType("jsonb");

//                entity.HasIndex(e => e.WorkflowId)
//                    .HasDatabaseName("IX_WorkflowInstances_WorkflowId");

//                entity.HasIndex(e => e.Status)
//                    .HasDatabaseName("IX_WorkflowInstances_Status");

//                entity.HasIndex(e => e.StartedAt)
//                    .HasDatabaseName("IX_WorkflowInstances_StartedAt");

//                entity.HasQueryFilter(w => !w.IsDeleted);
//            });

//            modelBuilder.Entity<WorkflowStepData>(entity =>
//            {
//                entity.ToTable("WorkflowStepInstances");
//                entity.HasKey(e => e.Id);

//                entity.Property(e => e.InputData)
//                    .HasColumnType("jsonb");

//                entity.Property(e => e.OutputData)
//                    .HasColumnType("jsonb");

//                entity.Property(e => e.StepVariables)
//                    .HasColumnType("jsonb");

//                entity.Property(e => e.Metadata)
//                    .HasColumnType("jsonb");

//                entity.HasIndex(e => new { e.WorkflowDataId, e.StepId })
//                    .HasDatabaseName("IX_WorkflowStepInstances_WorkflowInstance_Step");

//                entity.HasIndex(e => e.Status)
//                    .HasDatabaseName("IX_WorkflowStepInstances_Status");

//                entity.HasOne<WorkflowData>()
//                    .WithMany(w => w.StepInstances)
//                    .HasForeignKey(s => s.WorkflowDataId)
//                    .OnDelete(DeleteBehavior.Cascade);
//            });

//            modelBuilder.Entity<AuditLog>(entity =>
//            {
//                entity.ToTable("AuditLogs");
//                entity.HasKey(e => e.Id);

//                entity.Property(e => e.EntityType)
//                    .IsRequired()
//                    .HasMaxLength(100);

//                entity.Property(e => e.UserId)
//                    .HasMaxLength(100);

//                entity.Property(e => e.UserName)
//                    .HasMaxLength(200);

//                entity.Property(e => e.OldValues)
//                    .HasColumnType("jsonb");

//                entity.Property(e => e.NewValues)
//                    .HasColumnType("jsonb");

//                entity.Property(e => e.Metadata)
//                    .HasColumnType("jsonb");

//                entity.HasIndex(e => new { e.EntityType, e.EntityId })
//                    .HasDatabaseName("IX_AuditLogs_Entity");

//                entity.HasIndex(e => e.Timestamp)
//                    .HasDatabaseName("IX_AuditLogs_Timestamp");

//                entity.HasIndex(e => e.UserId)
//                    .HasDatabaseName("IX_AuditLogs_UserId");
//            });

//            modelBuilder.Entity<WorkflowCheckpoint>(entity =>
//            {
//                entity.ToTable("WorkflowCheckpoints");
//                entity.HasKey(e => e.Id);

//                entity.Property(e => e.Variables)
//                    .HasColumnType("jsonb");

//                entity.Property(e => e.StepData)
//                    .HasColumnType("jsonb");

//                entity.HasIndex(e => e.InstanceId)
//                    .HasDatabaseName("IX_WorkflowCheckpoints_InstanceId");

//                entity.HasIndex(e => e.CheckpointTime)
//                    .HasDatabaseName("IX_WorkflowCheckpoints_CheckpointTime");
//            });

//            modelBuilder.Entity<WorkflowRelation>(entity =>
//            {

//                entity.ToTable("WorkflowRelations");
//                entity.HasKey(e => e.Id);

//                entity.Property(e => e.Name)
//                    .HasMaxLength(200);

//                entity.Property(e => e.Metadata)
//                    .HasColumnType("jsonb");

//                entity.HasOne(e => e.ParentInstance)
//                    .WithMany(w => w.ChildRelations)
//                    .HasForeignKey(e => e.ParentInstanceId)
//                    .OnDelete(DeleteBehavior.Restrict);

//                entity.HasOne(e => e.ChildInstance)
//                    .WithMany(w => w.ParentRelations)
//                    .HasForeignKey(e => e.ChildInstanceId)
//                    .OnDelete(DeleteBehavior.Restrict);

//                entity.HasIndex(e => new { e.ParentInstanceId, e.RelationType })
//                    .HasDatabaseName("IX_WorkflowRelations_Parent");

//                entity.HasIndex(e => new { e.ChildInstanceId, e.RelationType })
//                    .HasDatabaseName("IX_WorkflowRelations_Child");
//            });
//        }
//    }
//}
