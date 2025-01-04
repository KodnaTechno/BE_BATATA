namespace AppWorkflow.Core.Interfaces.Services;

using AppWorkflow.Common.Exceptions;
using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Engine;
using System.Text;

public interface IWorkflowEventHandler
    {
        Task OnWorkflowStartedAsync(WorkflowData instance);
        Task OnWorkflowCompletedAsync(WorkflowData instance);
        Task OnStepStartedAsync(Guid instanceId, Guid stepId);
        Task OnStepCompletedAsync(Guid instanceId, Guid stepId, StepExecutionResult result);
        Task OnWorkflowErrorAsync(WorkflowEngineException exception);
        Task OnStepErrorAsync(Guid instanceId, Guid stepId, Exception exception);
    }