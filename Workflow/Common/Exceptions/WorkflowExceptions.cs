using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Common.Exceptions
{
    public class WorkflowException : Exception
    {
        public Guid? WorkflowId { get; }
        public Guid? InstanceId { get; }
        public Guid? StepId { get; }

        public WorkflowException(string message) : base(message) { }
        public WorkflowException(string message, Exception innerException) : base(message, innerException) { }
        public WorkflowException(string message, Guid workflowId, Guid? instanceId = null,Guid? stepId=null) : base(message)
        {
            WorkflowId = workflowId;
            InstanceId = instanceId;
            StepId = stepId;
        }
    }

    public class WorkflowNotFoundException : WorkflowException
    {
        public WorkflowNotFoundException(Guid workflowId)
            : base($"Workflow with ID {workflowId} not found", workflowId) { }

        public WorkflowNotFoundException(Guid workflowId, Guid? instanceId)
            : base($"Workflow instance {instanceId} for workflow {workflowId} not found", workflowId, instanceId) { }

        public WorkflowNotFoundException(string message) : base(message) { }
        public WorkflowNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class WorkflowExecutionException : WorkflowException
    {
        public Guid StepId { get; }

        public WorkflowExecutionException(string message, Guid workflowId, Guid instanceId, Guid stepId)
            : base(message, workflowId, instanceId)
        {
            StepId = stepId;
        }
    }

}
