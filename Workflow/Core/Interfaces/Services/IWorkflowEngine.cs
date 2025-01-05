namespace AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Common.Enums;
using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.Models;
using AppWorkflow.Engine;
using System.Text;

public interface IWorkflowEngine
    {
        Task<WorkflowData> StartWorkflowAsync(Guid workflowId, WorkflowModuleData moduleData, Dictionary<string, object> initialVariables = null);
    Task HandleApprovalTimeoutAsync(Guid workflowId, Guid stepId);
        Task<StepExecutionResult> ExecuteStepAsync(Guid instanceId, Guid stepId);
        Task<WorkflowStatus> GetWorkflowStatusAsync(Guid instanceId);
        Task PauseWorkflowAsync(Guid instanceId);
        Task ResumeWorkflowAsync(Guid instanceId);
        Task CancelWorkflowAsync(Guid instanceId);
        Task<WorkflowData> GetInstanceAsync(Guid instanceId);
        Task<IEnumerable<WorkflowData>> GetActiveInstancesAsync();
        Task RetryStepAsync(Guid instanceId, Guid stepId);
        Task SkipStepAsync(Guid instanceId, Guid stepId);
        Task RollbackStepAsync(Guid instanceId, Guid stepId);
        Task<IEnumerable<StepExecutionHistory>> GetStepHistoryAsync(Guid instanceId);
    }