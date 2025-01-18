namespace AppWorkflow.Core.Domain.Data;

using AppWorkflow.Common.Enums;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

public class WorkflowStepData
{
    public Guid Id { get; set; }
    public Guid WorkflowDataId { get; set; }
    public Guid StepId { get; set; }
    public StepStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string ErrorDetails { get; set; }
    public int RetryCount { get; set; }
    public Dictionary<string, object> StepVariables { get; set; } = new();
    public Dictionary<string, object> InputData { get; set; }
    public Dictionary<string, object> OutputData { get; set; }
    public TimeSpan? Duration => CompletedAt.HasValue ? CompletedAt.Value - StartedAt : null;
    public List<StepExecutionLog> ExecutionLogs { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new();

    // IEntity implementation
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }

    public class StepExecutionLog
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}