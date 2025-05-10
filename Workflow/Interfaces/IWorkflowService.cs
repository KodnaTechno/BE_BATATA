using AppWorkflow.Common.Enums;
using AppWorkflow.Core.Domain.Data;


namespace AppWorkflow.Core.Interfaces.Services;

public interface IWorkflowService
    {
        Task<WorkflowData> StartWorkflowAsync(Guid workflowId, dynamic moduleData);
        Task<WorkflowData> GetWorkflowDataAsync(Guid instanceId);
        Task<IEnumerable<WorkflowData>> GetActiveWorkflowsAsync();
        Task<WorkflowStatus> GetWorkflowStatusAsync(Guid instanceId);
        Task PauseWorkflowAsync(Guid instanceId);
        Task ResumeWorkflowAsync(Guid instanceId);
        Task CancelWorkflowAsync(Guid instanceId);
        Task<ValidationResult> ValidateWorkflowAsync(Workflow workflow);
    }