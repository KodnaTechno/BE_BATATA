using AppWorkflow.Core.Models;
using System.Text.Json;

namespace AppWorkflow.Infrastructure.Triggers;

/// <summary>
/// Context information for a workflow trigger
/// </summary>
public class TriggerContext
{
    public Guid WorkflowId { get; set; }
    public string? TriggerType { get; set; }
    public string? EventName { get; set; }
    public string? ApiRoute { get; set; }
    public string? Schedule { get; set; }
    public string? ModuleType { get; set; }
    public Guid ModuleId { get; set; }
    public WorkflowModuleData? Data { get; set; }
    public Dictionary<string, object>? Parameters { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}