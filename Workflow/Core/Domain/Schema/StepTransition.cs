namespace AppWorkflow.Core.Domain.Schema;

public class StepTransition
{
    public Guid Id { get; set; }
    public Guid SourceStepId { get; set; }
    public Guid TargetStepId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Condition { get; set; }
    public int Priority { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}