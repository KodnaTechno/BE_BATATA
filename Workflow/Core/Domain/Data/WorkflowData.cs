using AppWorkflow.Common.Enums;
using AppWorkflow.Core.Models;
using System.Text.Json;
using AppWorkflow.Core.Domain.Data;

namespace AppWorkflow.Domain.Data;

public class WorkflowData
{
    public Guid Id { get; set; }
    public Guid WorkflowId { get; set; }
    public string? WorkflowVersion { get; set; }
    public WorkflowStatus Status { get; set; }
    public Guid CurrentStepId { get; set; }
    public WorkflowModuleData? ModuleData { get; set; }
    public Dictionary<string, object> Variables { get; set; } = new();
    public List<WorkflowStepData> StepInstances { get; set; } = new();
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorDetails { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public virtual ICollection<WorkflowRelation> ParentRelations { get; set; } = new List<WorkflowRelation>();
    public virtual ICollection<WorkflowRelation> ChildRelations { get; set; } = new List<WorkflowRelation>();
}