namespace AppWorkflow.Core.Interfaces.Services;

using AppWorkflow.Engine;
using System.Text;

public interface IWorkflowMetrics
    {
        Task TrackWorkflowStarted(Guid workflowId, Guid instanceId);
        Task TrackWorkflowCompleted(Guid instanceId, TimeSpan duration);
        Task TrackStepExecution(Guid instanceId, Guid stepId, TimeSpan duration, bool success);
        Task TrackWorkflowError(Guid instanceId, Exception exception);
        Task<WorkflowMetrics> GetWorkflowMetricsAsync(Guid workflowId);
    }