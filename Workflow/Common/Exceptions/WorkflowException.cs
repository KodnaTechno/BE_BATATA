namespace AppWorkflow.Common.Exceptions;

public class WorkflowException : Exception
{
    public Guid? WorkflowId { get; }
    public Guid? InstanceId { get; }

    public WorkflowException(string message) : base(message)
    {
    }

    public WorkflowException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public WorkflowException(string message, Guid workflowId, Guid? instanceId = null) : base(message)
    {
        WorkflowId = workflowId;
        InstanceId = instanceId;
    }
}