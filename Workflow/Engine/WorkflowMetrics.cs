namespace AppWorkflow.Engine;

using System.Text;

public class WorkflowMetrics
    {
        public double AverageExecutionTime { get; set; }
        public double StepSuccessRate { get; set; }
        public int TotalExecutions { get; set; }
        public int FailedExecutions { get; set; }
        public Dictionary<string, double> StepExecutionTimes { get; set; }
        public Dictionary<string, int> StepFailureCounts { get; set; }
        public TimeSpan P95ExecutionTime { get; set; }
        public TimeSpan P99ExecutionTime { get; set; }
    }