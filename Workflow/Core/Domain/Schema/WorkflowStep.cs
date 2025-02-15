namespace AppWorkflow.Core.Domain.Schema;

public class WorkflowStep
{
    public Guid Id { get; set; }
    public Guid WorkflowId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public StepStatus Status { get; set; }
    public string ActionType { get; set; }
    public JsonDocument ActionConfiguration { get; set; }
    public List<StepTransition> Transitions { get; set; } = new();
    public TimeSpan? Timeout { get; set; }
    public RetryPolicy RetryPolicy { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public bool IsParallel { get; set; }
    public List<Guid> ParallelSteps { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
}