namespace AppWorkflow.Core.Interfaces.Services;


using System.Text;

public interface IWorkflowValidator
    {
        Task<ValidationResult> ValidateWorkflowAsync(Workflow workflow);
        Task<ValidationResult> ValidateStepAsync(WorkflowStep step);
        Task<bool> CanExecuteStepAsync(Guid instanceId, Guid stepId);
        Task<IEnumerable<ValidationError>> GetWorkflowErrorsAsync(Workflow workflow);
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<ValidationError> Errors { get; set; } = new();
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class ValidationError
    {
        public string Property { get; set; }
        public string Error { get; set; }
        public string Code { get; set; }
        public ValidationSeverity Severity { get; set; }
    }

    public enum ValidationSeverity
    {
        Error,
        Warning,
        Info
    }