using AppWorkflow.Core.Interfaces.Services;

namespace AppWorkflow.Common.Exceptions;

public class WorkflowValidationException : WorkflowException
{
    public IEnumerable<string> ValidationErrors { get; }

    public WorkflowValidationException(string message, IEnumerable<string> errors)
        : base(message)
    {
        ValidationErrors = errors;
    }

    public WorkflowValidationException(string message, string error)
        : this(message, new[] { error })
    {
    }

    public WorkflowValidationException(List<ValidationError> validationResultErrors) : base(String.Empty)
    {
        ValidationErrors = validationResultErrors.Select(e => e.Error);


    }
}