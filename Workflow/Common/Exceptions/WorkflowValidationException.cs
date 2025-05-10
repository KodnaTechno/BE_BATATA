namespace AppWorkflow.Common.Exceptions;

public class WorkflowValidationException : WorkflowException
{
    public IEnumerable<string> Errors { get; } = new List<string>();

    public WorkflowValidationException(string message) : base(message) { }
    public WorkflowValidationException(string message, Exception innerException) : base(message, innerException) { }
    public WorkflowValidationException(string message, Guid workflowId, Guid? instanceId = null)
        : base(message, workflowId, instanceId) { }
    public WorkflowValidationException(string message, IEnumerable<string> errors) : base(message)
    {
        Errors = errors;
    }
}
