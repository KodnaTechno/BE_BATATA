namespace AppWorkflow.Common.Exceptions;

public class WorkflowEngineException : WorkflowException
{
    public WorkflowEngineException(string message) : base(message) { }
    public WorkflowEngineException(string message, Exception innerException) : base(message, innerException) { }
    public WorkflowEngineException(string message, Guid workflowId, Guid? instanceId = null)
        : base(message, workflowId, instanceId) { }

    public WorkflowEngineException(string message, Guid workflowId, Guid? instanceId = null, Guid? id = default, Exception exception = null) : base(message, workflowId, instanceId, id)
    {
    }
}
