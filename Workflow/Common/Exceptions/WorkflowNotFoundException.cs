namespace AppWorkflow.Common.Exceptions;

public class WorkflowNotFoundException : WorkflowException
{
    public WorkflowNotFoundException(Guid workflowId)
        : base($"Workflow with ID {workflowId} not found", workflowId)
    {
    }

    public WorkflowNotFoundException(Guid workflowId, Guid? instanceId)
        : base($"Workflow instance {instanceId} for workflow {workflowId} not found", workflowId, instanceId)
    {
    }

    public WorkflowNotFoundException(string message) : base(message)
    {
    }

    public WorkflowNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}