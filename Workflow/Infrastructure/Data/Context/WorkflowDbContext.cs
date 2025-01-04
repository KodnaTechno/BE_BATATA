namespace AppWorkflow.Infrastructure.Data.Context;

using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.Domain.Schema;
using Microsoft.EntityFrameworkCore;

public class WorkflowDbContext : DbContext
    {
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowData> WorkflowDatas { get; set; }
        public DbSet<WorkflowStep> WorkflowSteps { get; set; }
        public DbSet<WorkflowStepData> WorkflowStepInstances { get; set; }
        public DbSet<WorkflowVariable> WorkflowVariables { get; set; }
        public DbSet<WorkflowCheckpoint> WorkflowCheckpoints { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SynchronizationPoint> SynchronizationPoints { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Workflow>(entity =>
            //{
            //    entity.ToTable("Workflows");
            //    entity.HasKey(e => e.Id);
            //    entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            //    entity.Property(e => e.TriggerConfigs).HasColumnType("jsonb");
            //    entity.Property(e => e.Metadata).HasColumnType("jsonb");

            //    entity.HasIndex(e => new { e.ModuleType, e.Status });
            //    entity.HasIndex(e => e.Version);
            //});

            modelBuilder.Entity<WorkflowStep>(entity =>
            {
                entity.ToTable("WorkflowSteps");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ActionConfiguration).HasColumnType("jsonb");
                entity.Property(e => e.Metadata).HasColumnType("jsonb");

                entity.HasOne<Workflow>()
                    .WithMany(w => w.Steps)
                    .HasForeignKey(s => s.WorkflowId);
            });

            // Add similar configurations for other entities...
        }
    }