namespace AppWorkflow.Core.Interfaces.Services;

using AppWorkflow.Infrastructure.Data.Context;
using System.Text;

public interface IWorkflowStateManager
    {
        Task SaveStateAsync(WorkflowExecutionContext context);
        Task<WorkflowExecutionContext> LoadStateAsync(Guid instanceId);
        Task<bool> HasActiveCheckpointAsync(Guid instanceId);
        Task CreateCheckpointAsync(Guid instanceId);
        Task RollbackToCheckpointAsync(Guid instanceId);
    }