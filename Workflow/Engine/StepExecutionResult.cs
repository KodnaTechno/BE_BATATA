namespace AppWorkflow.Engine;

using AppWorkflow.Common.Enums;
using System.Text;

public class StepExecutionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid? NextStepId { get; set; }
        public Exception Error { get; set; }
        public Dictionary<string, object> OutputVariables { get; set; } = new();
        public TimeSpan ExecutionTime { get; set; }
        public StepStatus Status { get; set; }
    }