namespace AppWorkflow.Engine;

using AppWorkflow.Common.Enums;
using System.Text;

public class StepExecutionHistory
    {
        public Guid StepId { get; set; }
        public string StepName { get; set; }
        public StepStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public TimeSpan? Duration => CompletedAt.HasValue ? CompletedAt - StartedAt : null;
        public int RetryCount { get; set; }
        public string ErrorDetails { get; set; }
        public Dictionary<string, object> InputData { get; set; }
        public Dictionary<string, object> OutputData { get; set; }
    }