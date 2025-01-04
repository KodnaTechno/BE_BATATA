namespace AppWorkflow.Common.Exceptions;


public class WorkflowEngineException : Exception
    {
        public Guid WorkflowId { get; }
        public Guid? InstanceId { get; }
        public Guid? StepId { get; }

        public WorkflowEngineException(string message, Guid workflowId, Guid? instanceId = null, Guid? stepId = null, Exception innerException = null)
            : base(message, innerException)
        {
            WorkflowId = workflowId;
            InstanceId = instanceId;
            StepId = stepId;
        }

    public WorkflowEngineException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}