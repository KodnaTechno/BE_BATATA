namespace AppWorkflow.Services.HealthCheck;

using System.Text;

public class WorkflowHealthStatus
    {
        public bool IsHealthy { get; set; }
       // public List<WorkflowHealthIssue> ActiveIssues { get; set; }
        public Dictionary<string, TimeSpan> StepPerformance { get; set; }
        public int FailedExecutionsLast24Hours { get; set; }
        public double SuccessRate { get; set; }
    }