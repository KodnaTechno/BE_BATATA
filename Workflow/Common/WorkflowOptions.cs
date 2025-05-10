namespace AppWorkflow.Common;

public class WorkflowOptions
{
    public int DefaultTimeoutMinutes { get; set; } = 60;
    public int MaxRetries { get; set; } = 3;
    public bool EnableTelemetry { get; set; } = true;
    // Add other workflow-related options as needed
}
