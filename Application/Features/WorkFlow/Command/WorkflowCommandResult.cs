namespace Application.Features.WorkFlow.Command;

using System;
using System.Collections.Generic;

public class WorkflowCommandResult
{
    public bool Success { get; set; }
    public object Result { get; set; }
    public Dictionary<string, object> OutputVariables { get; set; } = new();
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; }
    public Exception Exception { get; set; }
    public Dictionary<string, object> CheckpointData { get; set; } = new();
    public Guid? WorkflowInstanceId { get; set; }
    public bool CanRetry { get; set; }
    public Dictionary<string, object> DiagnosticInfo { get; set; } = new();
    public TimeSpan? ExecutionTime { get; set; }
}
