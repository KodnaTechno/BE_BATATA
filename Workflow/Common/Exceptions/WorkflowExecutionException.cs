namespace AppWorkflow.Common.Exceptions;

public class WorkflowExecutionException : WorkflowException
{
    public Guid StepId { get; }

    public WorkflowExecutionException(string message, Guid workflowId, Guid instanceId, Guid stepId)
        : base(message, workflowId, instanceId)
    {
        StepId = stepId;
    }

 
}