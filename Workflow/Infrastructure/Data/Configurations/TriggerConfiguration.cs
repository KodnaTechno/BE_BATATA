using System.Text.Json;

namespace AppWorkflow.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for a workflow trigger
/// </summary>
public class TriggerConfiguration
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WorkflowId { get; set; }
    public string Type { get; set; }
    
    // Event trigger properties
    public string EventName { get; set; }
    
    // Schedule trigger properties
    public string Schedule { get; set; }
    
    // API trigger properties
    public string ApiRoute { get; set; }
    
    // Filter conditions
    public Dictionary<string, object> Conditions { get; set; } = new();
    
    // Additional configuration as JSON
    public JsonDocument Configuration { get; set; }
    
    // Metadata for the trigger
    public Dictionary<string, string> Metadata { get; set; } = new();
}