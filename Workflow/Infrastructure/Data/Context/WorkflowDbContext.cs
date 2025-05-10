
using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.Models;
using AppWorkflow.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using static AppWorkflow.Core.Domain.Data.WorkflowStepData;

namespace AppWorkflow.Infrastructure.Data.Context;
public class WorkflowDbContext : DbContext
    {
    public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : base(options)
    {
    }
   
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
        public DbSet<ApprovalHistory> ApprovalHistories { get; set; }
        public DbSet<WorkflowData> WorkflowData { get; set; }
        public DbSet<WorkflowStep> WorkflowSteps { get; set; }
        public DbSet<WorkflowStepData> WorkflowStepData { get; set; }
        public DbSet<WorkflowCheckpoint> WorkflowCheckpoints { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SynchronizationPoint> SynchronizationPoints { get; set; }
        public DbSet<StepExecutionLog> StepExecutionLogs { get; set; }
        public DbSet<StepTransition> StepTransitions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
             base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new WorkflowConfiguration());
        modelBuilder.ApplyConfiguration(new WorkflowStepConfiguration());
        modelBuilder.ApplyConfiguration(new WorkflowDataConfiguration());
        modelBuilder.ApplyConfiguration(new WorkflowStepDataConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new WorkflowCheckpointConfiguration());
        modelBuilder.ApplyConfiguration(new StepExecutionLogConfiguration());
        modelBuilder.ApplyConfiguration(new StepTransitionConfiguration());
        modelBuilder.ApplyConfiguration(new WorkflowRelationConfiguration());
        modelBuilder.ApplyConfiguration(new ApprovalConfiguration());
        modelBuilder.ApplyConfiguration(new ApprovalHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new SynchronizationPointConfiguration());
        // Add similar configurations for other entities...
    }
    }
