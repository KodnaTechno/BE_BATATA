namespace AppWorkflow.Core.Domain.Schema;

using AppWorkflow.Infrastructure.Data.Configurations;
using AppWorkflow.Triggers;

public class Workflow
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public WorkflowStatus Status { get; set; }
    public string ModuleType { get; set; }
    public Guid InitialStepId { get; set; }
    public string Version { get; set; }
    public bool IsLatestVersion { get; set; }
    public List<WorkflowVariable> Variables { get; set; } = new();
    public List<WorkflowStep> Steps { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new();
    public TimeSpan? Timeout { get; set; }
    public RetryPolicy RetryPolicy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public string AuditLog { get; set; }
    public List<TriggerConfiguration> TriggerConfigs { get; set; }

}