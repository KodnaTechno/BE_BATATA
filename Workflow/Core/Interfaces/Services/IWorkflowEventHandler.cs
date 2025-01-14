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

public class WorkflowEventHandler : IWorkflowEventHandler
{
    public Task OnStepCompletedAsync(Guid instanceId, Guid stepId, StepExecutionResult result)
    {
        throw new NotImplementedException();
    }

    public Task OnStepErrorAsync(Guid instanceId, Guid stepId, Exception exception)
    {
        throw new NotImplementedException();
    }

    public Task OnStepStartedAsync(Guid instanceId, Guid stepId)
    {
        throw new NotImplementedException();
    }

    public Task OnWorkflowCompletedAsync(WorkflowData instance)
    {
        throw new NotImplementedException();
    }

    public Task OnWorkflowErrorAsync(WorkflowEngineException exception)
    {
        throw new NotImplementedException();
    }

    public Task OnWorkflowStartedAsync(WorkflowData instance)
    {
        throw new NotImplementedException();
    }
}