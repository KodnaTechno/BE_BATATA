namespace Application.Features.WorkFlow.Command;

using System.Collections.Generic;

public class WorkflowCommandResult
{
    public bool Success { get; set; }
    public object Result { get; set; }
    public Dictionary<string, object> OutputVariables { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}
