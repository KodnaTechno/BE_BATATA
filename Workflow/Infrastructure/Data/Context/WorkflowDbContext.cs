
using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.Domain.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppWorkflow.Infrastructure.Data.Context;
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


    public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureWorkflowRelations();
    }
}