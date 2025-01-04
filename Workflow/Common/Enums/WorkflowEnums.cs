namespace AppWorkflow.Common.Enums;

public enum WorkflowStatus
    {
        Draft,
        Active,
        Suspended,
        Completed,
        Terminated,
        Failed,
        TimedOut
    }
    public enum StepStatus
    {
        Pending,
        InProgress,
        Completed,
        Failed,
        Skipped,
        TimedOut,
        Cancelled
    }

    public enum VariableType
    {
        String,
        Number,
        Boolean,
        DateTime,
        Array,
        Object
    }
    public enum AuditAction
    {
        Created,
        Updated,
        Deleted,
        StatusChanged,
        PropertyChanged,
        StepCompleted,
        WorkflowStarted,
        WorkflowCompleted,
        WorkflowCancelled,
        Error,
        Custom
    }
    public enum WorkflowRelationType
    {
        SubWorkflow,
        Reference,
        Dependent
    }