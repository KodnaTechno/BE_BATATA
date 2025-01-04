namespace AppWorkflow.Core.Interfaces.Services;

using AppWorkflow.Services.HealthCheck;
using System.Text;

public interface IWorkflowHealthMonitor
    {
        Task<WorkflowHealthStatus> GetWorkflowHealthAsync(Guid workflowId);
        //Task<IEnumerable<WorkflowHealthIssue>> GetActiveIssuesAsync();
        //Task ReportHealthIssueAsync(WorkflowHealthIssue issue);
        //Task<WorkflowPerformanceMetrics> GetPerformanceMetricsAsync(Guid workflowId);
    }